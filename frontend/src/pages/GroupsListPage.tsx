import React, { useEffect, useState, KeyboardEvent } from 'react';
import { getGroups, createGroup, GroupDto } from '../services/api';
import { Box, Typography, TextField, Button, Card, CardContent } from '@mui/material';
import Grid from '@mui/material/Grid';

export default function GroupsListPage() {
  const [groups, setGroups] = useState<GroupDto[]>([]);
  const [newTitle, setNewTitle] = useState<string>('');

  useEffect(() => { fetchGroups(); }, []);

  const fetchGroups = async () => {
    try {
      const data = await getGroups();
      setGroups(data);
    } catch (error) {
      console.error('Failed to load groups', error);
    }
  };

  const handleCreate = async () => {
    const title = newTitle.trim();
    if (!title) return;
    try {
      await createGroup(title);
      setNewTitle('');
      fetchGroups();
    } catch (error) {
      console.error('Failed to create group', error);
    }
  };

  // Create on Enter key press
  const handleKeyDown = (e: KeyboardEvent<HTMLDivElement>) => {
    if (e.key === 'Enter') {
      handleCreate();
    }
  };

  return (
    <Box p={4}>
      <Typography variant="h4" gutterBottom>
        All Groups
      </Typography>
      <Box display="flex" mb={3}>
        <TextField
          label="New Group"
          value={newTitle}
          onChange={e => setNewTitle(e.target.value)}
          onKeyDown={handleKeyDown}  // Trigger create on Enter
          fullWidth
        />
        <Button variant="contained" onClick={handleCreate} sx={{ ml: 2 }}>
          Create
        </Button>
      </Box>
      <Grid container spacing={2}>
        {groups.map(g => (
          <Grid size={{ xs: 12, sm: 6, md: 4 }} key={g.id}>
            <Card>
              <CardContent>
                <Typography variant="h6">{g.title}</Typography>
                <Typography>
                  Balance: {g.balance >= 0 ? '+' : '-'}{Math.abs(g.balance).toFixed(2)}
                </Typography>
                <Button href={`/groups/${g.id}`} size="small">
                  View
                </Button>
              </CardContent>
            </Card>
          </Grid>
        ))}
      </Grid>
    </Box>
  );
}
