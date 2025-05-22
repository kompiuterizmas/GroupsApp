import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import {
  getUsers,
  getUserTransactions,
  UserDto
} from '../services/api';
import {
  Box,
  Typography,
  Card,
  CardContent,
  CircularProgress
} from '@mui/material';
import Grid from '@mui/material/Grid';

interface UserWithBalance extends UserDto {
  balance: number;
}

export default function MembersPage() {
  const [users, setUsers] = useState<UserWithBalance[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(false);

  useEffect(() => {
    (async () => {
      try {
        const list = await getUsers();
        // compute each user's total balance
        const withBal = await Promise.all(
          list.map(async u => {
            const txs = await getUserTransactions(u.id);
            const bal = txs.reduce((sum, tx) => {
              const share = tx.splitDetails?.[u.id] ?? 0;
              return sum + (tx.payerId === u.id
                ? tx.amount - share
                : -share);
            }, 0);
            return { ...u, balance: bal };
          })
        );
        setUsers(withBal);
      } catch {
        setError(true);
      } finally {
        setLoading(false);
      }
    })();
  }, []);

  if (loading) {
    return (
      <Box p={4} textAlign="center">
        <CircularProgress />
      </Box>
    );
  }
  if (error) {
    return (
      <Box p={4}>
        <Typography color="error">Failed to load users.</Typography>
      </Box>
    );
  }

  return (
    <Box p={4} sx={{ width: '100%' }}>
      <Typography variant="h4" gutterBottom>
        All Users
      </Typography>
      <Grid container spacing={2}>
        {users.map(u => (
          <Grid
            size={{ xs: 12, sm: 6, md: 4 }}
            key={u.id}
            component={Link}
            to={`/users/${u.id}`}
          >
            <Card>
              <CardContent>
                <Typography>ID: {u.id}</Typography>
                <Typography>Name: {u.name}</Typography>
                <Typography>Balance: {u.balance.toFixed(2)}</Typography>
              </CardContent>
            </Card>
          </Grid>
        ))}
      </Grid>
    </Box>
  );
}
