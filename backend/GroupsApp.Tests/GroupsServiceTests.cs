using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Xunit;
using GroupsApp.Api.Data;
using GroupsApp.Api.Services;
using GroupsApp.Api.DTOs;
using GroupsApp.Api.Mappings;

namespace GroupsApp.Tests
{
    public class GroupsServiceTests
    {
        private AppDbContext CreateInMemoryDb()
        {
            var opts = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
                .Options;
            return new AppDbContext(opts);
        }

        private IMapper CreateMapper()
        {
            var cfg = new MapperConfiguration(c => c.AddProfile<MappingProfile>());
            return cfg.CreateMapper();
        }

        [Fact]
        public async Task GetAllAsync_ReturnsCreatedGroups()
        {
            var db = CreateInMemoryDb();
            var mapper = CreateMapper();
            var svc = new GroupsService(db, mapper);

            await svc.CreateAsync(new CreateGroupDto { Title = "G1" });
            await svc.CreateAsync(new CreateGroupDto { Title = "G2" });

            var all = (await svc.GetAllAsync()).ToList();

            Assert.Equal(2, all.Count);
            Assert.Contains(all, g => g.Title == "G1");
            Assert.Contains(all, g => g.Title == "G2");
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsDetailWithMembersAndTransactions()
        {
            var db = CreateInMemoryDb();
            var mapper = CreateMapper();
            var svc = new GroupsService(db, mapper);

            var g = await svc.CreateAsync(new CreateGroupDto { Title = "Trip" });
            int gid = g.Id;
            await svc.AddMemberAsync(gid, new CreateMemberDto { Name = "A" });
            await svc.AddMemberAsync(gid, new CreateMemberDto { Name = "B" });
            await svc.CreateTransactionAsync(gid, new CreateTransactionDto {
                PayerId = (await svc.GetMembersAsync(gid)).First().Id,
                Amount = 100,
                SplitType = SplitType.Equal
            });

            var detail = await svc.GetByIdAsync(gid);

            Assert.Equal("Trip", detail.Title);
            Assert.Equal(2, detail.Members.Count);
            Assert.Single(detail.Transactions);
        }

        [Fact]
        public async Task GetMembersAsync_ReturnsOnlyGroupMembers()
        {
            var db = CreateInMemoryDb();
            var mapper = CreateMapper();
            var svc = new GroupsService(db, mapper);

            var g1 = await svc.CreateAsync(new CreateGroupDto { Title = "G1" });
            var g2 = await svc.CreateAsync(new CreateGroupDto { Title = "G2" });
            await svc.AddMemberAsync(g1.Id, new CreateMemberDto { Name = "X" });
            await svc.AddMemberAsync(g2.Id, new CreateMemberDto { Name = "Y" });

            var membersG1 = (await svc.GetMembersAsync(g1.Id)).ToList();
            var membersG2 = (await svc.GetMembersAsync(g2.Id)).ToList();

            Assert.Single(membersG1);
            Assert.Equal("X", membersG1[0].Name);
            Assert.Single(membersG2);
            Assert.Equal("Y", membersG2[0].Name);
        }

        [Fact]
        public async Task GetGroupTransactionsAsync_ReturnsOnlyGroupTransactions()
        {
            var db = CreateInMemoryDb();
            var mapper = CreateMapper();
            var svc = new GroupsService(db, mapper);

            var g1 = await svc.CreateAsync(new CreateGroupDto { Title = "G1" });
            var g2 = await svc.CreateAsync(new CreateGroupDto { Title = "G2" });
            await svc.AddMemberAsync(g1.Id, new CreateMemberDto { Name = "A" });
            await svc.AddMemberAsync(g2.Id, new CreateMemberDto { Name = "B" });

            var aId = (await svc.GetMembersAsync(g1.Id)).First().Id;
            var bId = (await svc.GetMembersAsync(g2.Id)).First().Id;

            await svc.CreateTransactionAsync(g1.Id, new CreateTransactionDto { PayerId = aId, Amount = 50, SplitType = SplitType.Equal });
            await svc.CreateTransactionAsync(g2.Id, new CreateTransactionDto { PayerId = bId, Amount = 75, SplitType = SplitType.Equal });

            var txs1 = (await svc.GetGroupTransactionsAsync(g1.Id)).ToList();
            var txs2 = (await svc.GetGroupTransactionsAsync(g2.Id)).ToList();

            Assert.Single(txs1);
            Assert.Equal(50m, txs1[0].Amount);
            Assert.Single(txs2);
            Assert.Equal(75m, txs2[0].Amount);
        }

        [Fact]
        public async Task RemoveMemberAsync_ShouldRemoveMember()
        {
            var db = CreateInMemoryDb();
            var mapper = CreateMapper();
            var svc = new GroupsService(db, mapper);

            var g = await svc.CreateAsync(new CreateGroupDto { Title = "G" });
            int gid = g.Id;
            await svc.AddMemberAsync(gid, new CreateMemberDto { Name = "ToRemove" });
            var before = await svc.GetMembersAsync(gid);
            Assert.Single(before);

            await svc.RemoveMemberAsync(gid, before.First().Id);

            var after = await svc.GetMembersAsync(gid);
            Assert.Empty(after);
        }

        [Fact]
        public async Task CreateTransactionAsync_PercentageSplit_UpdatesBalances()
        {
            var db = CreateInMemoryDb();
            var mapper = CreateMapper();
            var svc = new GroupsService(db, mapper);

            var g  = await svc.CreateAsync(new CreateGroupDto { Title = "G" });
            int gid = g.Id;
            await svc.AddMemberAsync(gid, new CreateMemberDto { Name = "A" });
            await svc.AddMemberAsync(gid, new CreateMemberDto { Name = "B" });
            var mm = (await svc.GetMembersAsync(gid)).ToList();
            var payload = new CreateTransactionDto {
                PayerId     = mm[0].Id,
                Amount      = 100m,
                SplitType   = SplitType.Percentage,
                SplitDetails= new Dictionary<int, decimal> {
                    [mm[0].Id] = 30m,
                    [mm[1].Id] = 70m
                }
            };

            await svc.CreateTransactionAsync(gid, payload);
            var detail = await svc.GetByIdAsync(gid);
            var aBal = detail.Members.First(m => m.Id == mm[0].Id).Balance;
            var bBal = detail.Members.First(m => m.Id == mm[1].Id).Balance;

            Assert.Equal(70m, aBal);   // A paid 100, his share=30 => +70
            Assert.Equal(-70m, bBal);  // B owes 70
        }

        [Fact]
        public async Task CreateTransactionAsync_ManualSplit_UpdatesBalances()
        {
            var db = CreateInMemoryDb();
            var mapper = CreateMapper();
            var svc = new GroupsService(db, mapper);

            var g  = await svc.CreateAsync(new CreateGroupDto { Title = "G" });
            int gid = g.Id;
            await svc.AddMemberAsync(gid, new CreateMemberDto { Name = "A" });
            await svc.AddMemberAsync(gid, new CreateMemberDto { Name = "B" });
            var mm = (await svc.GetMembersAsync(gid)).ToList();
            var payload = new CreateTransactionDto {
                PayerId     = mm[0].Id,
                Amount      = 150m,
                SplitType   = SplitType.Manual,
                SplitDetails= new Dictionary<int, decimal> {
                    [mm[0].Id] = 40m,
                    [mm[1].Id] = 110m
                }
            };

            await svc.CreateTransactionAsync(gid, payload);
            var detail = await svc.GetByIdAsync(gid);
            var aBal = detail.Members.First(m => m.Id == mm[0].Id).Balance;
            var bBal = detail.Members.First(m => m.Id == mm[1].Id).Balance;

            Assert.Equal(110m, aBal);
            Assert.Equal(-110m, bBal);
        }

        [Fact]
        public async Task CreateTransactionAsync_InvalidPayer_Throws()
        {
            var db = CreateInMemoryDb();
            var mapper = CreateMapper();
            var svc = new GroupsService(db, mapper);

            var g = await svc.CreateAsync(new CreateGroupDto { Title = "G" });
            int gid = g.Id;

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                svc.CreateTransactionAsync(gid, new CreateTransactionDto {
                    PayerId = 999, Amount = 50m, SplitType = SplitType.Equal
                })
            );
        }
    }
}
