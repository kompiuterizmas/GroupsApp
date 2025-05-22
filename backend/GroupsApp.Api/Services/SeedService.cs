using System;
using System.Collections.Generic;
using System.Linq;
using GroupsApp.Api.Data;
using GroupsApp.Api.Models;
using Microsoft.Extensions.Hosting;

namespace GroupsApp.Api.Services
{
    public interface ISeedService
    {
        void SeedDemoData();
    }

    public class SeedService : ISeedService
    {
        private readonly AppDbContext _db;
        private readonly IHostEnvironment _env;
        private static readonly Random _rnd = new();

        public SeedService(AppDbContext db, IHostEnvironment env)
        {
            _db  = db;
            _env = env;
        }

        public void SeedDemoData()
        {
            if (!_env.IsDevelopment())
                throw new InvalidOperationException("Seeding allowed only in Development.");

            if (_db.Groups.Any())
                return;

            // 1) Users
            var users = Enumerable.Range(1, 50)
                .Select(i => new User { Name = $"User{i:00}" })
                .ToList();
            _db.Users.AddRange(users);
            _db.SaveChanges();

            // Preload SplitType enum values
            var splitTypes = Enum.GetValues(typeof(SplitType))
                                 .Cast<SplitType>()
                                 .ToArray();

            // 2) Groups → Members → Transactions
            for (int g = 1; g <= 10; g++)
            {
                var group = new Group { Title = $"Group {g:00}" };
                _db.Groups.Add(group);
                _db.SaveChanges();

                var memberCount = _rnd.Next(5, 15);
                var members = users.OrderBy(_ => _rnd.Next()).Take(memberCount).ToList();

                foreach (var u in members)
                {
                    _db.GroupMembers.Add(new GroupMember
                    {
                        GroupId = group.Id,
                        UserId  = u.Id
                    });
                }
                _db.SaveChanges();

                int txCount = _rnd.Next(5, 20);
                for (int t = 0; t < txCount; t++)
                {
                    var payer  = members[_rnd.Next(members.Count)];
                    var amount = Math.Round((decimal)(_rnd.NextDouble() * 490 + 10), 2);
                    var splits = members.ToDictionary(m => m.Id, _ => Math.Round(amount / members.Count, 2));
                    var type   = splitTypes[_rnd.Next(splitTypes.Length)];

                    var tx = new Transaction
                    {
                        GroupId      = group.Id,
                        PayerUserId  = payer.Id,
                        Description  = $"Payment {t+1} in {group.Title}",
                        SplitType    = type,
                        Amount       = amount,
                        Date         = DateTime.UtcNow.AddDays(-_rnd.Next(0, 30)),
                        SplitDetails = splits
                    };
                    _db.Transactions.Add(tx);
                }
                _db.SaveChanges();
            }
        }
    }
}
