import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { getGroups, GroupDto } from '../services/api';
import { Box, Typography, Card, CardContent } from '@mui/material';
import Grid from '@mui/material/Grid';

export default function GroupsListPage() {
  const [groups, setGroups] = useState<GroupDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(false);

  useEffect(() => {
    (async () => {
      try {
        const data = await getGroups();
        setGroups(data);
      } catch {
        setError(true);
      } finally {
        setLoading(false);
      }
    })();
  }, []);

  if (loading) {
    return <Typography align="center">Loading groups…</Typography>;
  }
  if (error) {
    return <Typography color="error">Failed to load groups.</Typography>;
  }

  return (
    <Box p={4} sx={{ width: '100%' }}>
      <Typography variant="h4" gutterBottom>
        All Groups
      </Typography>
      <Grid container spacing={2}>
        {groups.map(g => (
          <Grid
            size={{ xs: 12, sm: 6, md: 4 }}
            key={g.id}
            component={Link}
            to={`/groups/${g.id}`}
          >
            <Card>
              <CardContent>
                <Typography variant="h6">{g.title}</Typography>
                <Typography>Balance: {g.balance.toFixed(2)}</Typography>
              </CardContent>
            </Card>
          </Grid>
        ))}
      </Grid>
    </Box>
  );
}
