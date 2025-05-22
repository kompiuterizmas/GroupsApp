import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import {
  getGroupDetail,
  GroupDetailDto,
  MemberDto
} from '../services/api';
import {
  Box,
  Typography,
  Card,
  CardContent,
  Grid,
  CircularProgress
} from '@mui/material';
import NetworkErrorPage from '../components/NetworkErrorPage';
import TransactionCard from '../components/TransactionCard';

export default function GroupDetailPage() {
  const { groupId } = useParams<{ groupId: string }>();
  const [group, setGroup] = useState<GroupDetailDto | null>(null);
  const [error, setError] = useState(false);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!groupId) return;
    (async () => {
      try {
        const data = await getGroupDetail(+groupId);
        setGroup(data);
      } catch {
        setError(true);
      } finally {
        setLoading(false);
      }
    })();
  }, [groupId]);

  if (loading) {
    return (
      <Box p={4} textAlign="center">
        <CircularProgress />
      </Box>
    );
  }
  if (error || !group) {
    return <NetworkErrorPage onRetry={() => window.location.reload()} />;
  }

  return (
    <Box p={4} sx={{ maxWidth: 800, mx: 'auto' }}>
      <Typography variant="h4" gutterBottom>
        {group.title}
      </Typography>

      <Typography variant="h6" gutterBottom>
        Members
      </Typography>
      <ul>
        {group.members.map((m: MemberDto) => (
          <li key={m.id}>
            {m.name} (Balance: {m.balance.toFixed(2)})
          </li>
        ))}
      </ul>

      <Typography variant="h6" gutterBottom sx={{ mt: 3 }}>
        Transactions
      </Typography>
      <Grid container spacing={2}>
         {group.transactions.map(tx => {
         const payer = group.members.find(m => m.id === tx.payerId);
         const payerName = payer?.name ?? `#${tx.payerId}`;
         return (
           <Grid size={{ xs: 12, sm: 6, md: 4 }} key={tx.id}>
             <TransactionCard
               transaction={tx}
               groupName={group.title}
               payerName={payerName}
             />
           </Grid>
         );
       })}
      </Grid>
    </Box>
  );
}
