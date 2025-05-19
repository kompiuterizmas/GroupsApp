import React, { useEffect, useState } from 'react';
import { getGroups, GroupDto, getGroupDetail, MemberDto } from '../services/api';
import { Box, Typography, Card, CardContent } from '@mui/material';
import Grid from '@mui/material/Grid';

export default function MembersPage() {
  const [members, setMembers] = useState<MemberDto[]>([]);

  useEffect(() => {
    async function fetchAllMembers() {
      try {
        const groups: GroupDto[] = await getGroups();
        const allMembers: MemberDto[] = [];
        for (const g of groups) {
          const detail = await getGroupDetail(g.id);
          allMembers.push(...detail.members.map(m => ({ ...m })));
        }
        setMembers(allMembers);
      } catch (err) {
        console.error('Failed to load members', err);
      }
    }
    fetchAllMembers();
  }, []);

  return (
    <Box p={4} sx={{ width: '100%' }}>
      <Typography variant="h4" gutterBottom>
        All Members
      </Typography>
      <Grid container spacing={2}>
        {members.map(m => (
          <Grid size={{ xs: 12, sm: 6, md: 4 }} key={m.id}>
            <Card>
              <CardContent>
                <Typography>Name: {m.name}</Typography>
                <Typography>Balance: {m.balance}</Typography>
              </CardContent>
            </Card>
          </Grid>
        ))}
      </Grid>
    </Box>
    );
}