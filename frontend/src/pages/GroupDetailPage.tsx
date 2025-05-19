// frontend/src/pages/GroupDetailPage.tsx

import React, { useEffect, useState, KeyboardEvent } from 'react';
import { useParams, Link } from 'react-router-dom';
import {
  getGroupDetail,
  addMember,
  GroupDetailDto,
  MemberDto,
  TransactionDto
} from '../services/api';
import {
  Typography,
  TextField,
  Button,
  Card,
  CardContent,
  Box
} from '@mui/material';
import NetworkErrorPage from '../components/NetworkErrorPage';

export default function GroupDetailPage() {
  const { groupId } = useParams<{ groupId: string }>();
  const [group, setGroup] = useState<GroupDetailDto | null>(null);
  const [memberName, setMemberName] = useState('');
  const [hasError, setHasError] = useState(false);

  useEffect(() => {
    if (groupId) load();
  }, [groupId]);

  const load = async () => {
    setHasError(false);
    try {
      const data = await getGroupDetail(+groupId!);
      setGroup(data);
    } catch {
      setHasError(true);
    }
  };

  const handleAdd = async () => {
    if (!memberName.trim()) return;
    try {
      await addMember(+groupId!, memberName.trim());
      setMemberName('');
      load();
    } catch {
      setHasError(true);
    }
  };

  // New: handle Enter key in the input field
  const handleKeyDown = (e: KeyboardEvent<HTMLDivElement>) => {
    if (e.key === 'Enter') {
      e.preventDefault();
      handleAdd();
    }
  };

  if (hasError) {
    return <NetworkErrorPage onRetry={load} />;
  }

  if (!group) {
    return <Typography>Loading...</Typography>;
  }

  return (
    <Box sx={{ p: 4, display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
      <Typography variant="h4" gutterBottom>
        {group.title}
      </Typography>

      <Box component="form" sx={{ mb: 3, width: '100%', maxWidth: 400 }}>
        <TextField
          fullWidth
          label="New Member Name"
          value={memberName}
          onChange={e => setMemberName(e.target.value)}
          onKeyDown={handleKeyDown}           // ← added
          margin="normal"
        />
        <Button
          variant="contained"
          onClick={handleAdd}
          disabled={!memberName.trim()}
          fullWidth
        >
          Add Member
        </Button>
      </Box>

      <Typography variant="h6">Members</Typography>
      {group.members.map(m => (
        <Card key={m.id} sx={{ width: '100%', maxWidth: 400, mb: 1 }}>
          <CardContent>
            <Typography>{m.name}</Typography>
            <Typography>
              Balance: {m.balance >= 0 ? '+' : '-'}
              {Math.abs(m.balance).toFixed(2)}
            </Typography>
          </CardContent>
        </Card>
      ))}

      <Typography variant="h6" sx={{ mt: 4 }}>
        Transactions
      </Typography>
      {group.transactions.map(tx => (
        <Card key={tx.id} sx={{ width: '100%', maxWidth: 400, mb: 1 }}>
          <CardContent>
            <Typography>
              Paid by: {tx.payerId}, Amount: {tx.amount}, Date:{' '}
              {new Date(tx.date).toLocaleString()}
            </Typography>
          </CardContent>
        </Card>
      ))}

      <Button
        component={Link}
        to={`/groups/${groupId}/new-transaction`}
        variant="contained"
        sx={{ mt: 4 }}
      >
        New Transaction
      </Button>
    </Box>
  );
}
