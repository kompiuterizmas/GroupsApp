// frontend/src/pages/NewTransactionPage.tsx

import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  getGroupDetail,
  createTransaction,
  GroupDetailDto,
  MemberDto,
  CreateTransactionPayload
} from '../services/api';
import {
  Box,
  Typography,
  TextField,
  Button,
  RadioGroup,
  FormControlLabel,
  Radio,
  MenuItem
} from '@mui/material';
import NetworkErrorPage from '../components/NetworkErrorPage';

export default function NewTransactionPage() {
  const { groupId } = useParams<{ groupId: string }>();
  const navigate = useNavigate();

  const [group, setGroup] = useState<GroupDetailDto | null>(null);
  const [payerId, setPayerId] = useState<number>();
  const [amount, setAmount] = useState<number>(0);
  const [splitType, setSplitType] = useState<'Equal' | 'Percentage' | 'Manual'>('Equal');
  const [splitDetails, setSplitDetails] = useState<Record<number, number>>({});
  const [hasError, setHasError] = useState(false);

  useEffect(() => {
    if (!groupId) return;
    setHasError(false);
    getGroupDetail(+groupId)
      .then(data => {
        setGroup(data);
        if (data.members.length > 0) {
          setPayerId(data.members[0].id);
          // initialize splitDetails with equal shares
          const equalShare = parseFloat((data.members.length ? (0).toString() : '0'));
          setSplitDetails(
            data.members.reduce((acc, m) => {
              acc[m.id] = 0;
              return acc;
            }, {} as Record<number, number>)
          );
        }
      })
      .catch(() => setHasError(true));
  }, [groupId]);

  const handleKeyDown = (e: React.KeyboardEvent<HTMLFormElement>) => {
    if (e.key === 'Enter') {
      e.preventDefault();
      handleSubmit();
    }
  };

  const handleSubmit = async () => {
    if (!groupId || payerId === undefined) return;
    setHasError(false);
    const payload: CreateTransactionPayload = {
      payerId,
      amount,
      splitType,
      splitDetails: splitType === 'Equal' ? undefined : splitDetails
    };
    try {
      await createTransaction(+groupId, payload);
      navigate(`/groups/${groupId}`);
    } catch {
      setHasError(true);
    }
  };

  if (hasError) {
    return <NetworkErrorPage onRetry={() => navigate(0)} />;
  }

  if (!group || payerId === undefined) {
    return <Typography>Loading...</Typography>;
  }

  // Helper to render split inputs
  const renderSplitInputs = () => {
    if (splitType === 'Equal') return null;
    return group.members.map((m: MemberDto) => (
      <Box key={m.id} sx={{ mb: 2 }}>
        <TextField
          fullWidth
          label={
            splitType === 'Percentage'
              ? `${m.name} (%)`
              : `${m.name} (amount)`
          }
          type="number"
          value={splitDetails[m.id] ?? ''}
          onChange={e => {
            const value = parseFloat(e.target.value) || 0;
            setSplitDetails(prev => ({ ...prev, [m.id]: value }));
          }}
        />
      </Box>
    ));
  };

  return (
    <Box
      component="form"
      onKeyDown={handleKeyDown}
      sx={{
        p: 4,
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        width: '100%',
        maxWidth: 500,
        mx: 'auto'
      }}
    >
      <Typography variant="h4" gutterBottom>
        New Transaction in "{group.title}"
      </Typography>

      <TextField
        select
        fullWidth
        label="Payer"
        value={payerId}
        onChange={e => setPayerId(+e.target.value)}
        sx={{ mb: 2 }}
      >
        {group.members.map(m => (
          <MenuItem key={m.id} value={m.id}>
            {m.name}
          </MenuItem>
        ))}
      </TextField>

      <TextField
        fullWidth
        label="Amount"
        type="number"
        value={amount}
        onChange={e => setAmount(parseFloat(e.target.value) || 0)}
        sx={{ mb: 2 }}
      />

      <Typography variant="subtitle1" gutterBottom>
        Split Type
      </Typography>
      <RadioGroup
        row
        value={splitType}
        onChange={e =>
          setSplitType(e.target.value as 'Equal' | 'Percentage' | 'Manual')
        }
        sx={{ mb: 2 }}
      >
        <FormControlLabel value="Equal" control={<Radio />} label="Equal" />
        <FormControlLabel
          value="Percentage"
          control={<Radio />}
          label="Percentage"
        />
        <FormControlLabel value="Manual" control={<Radio />} label="Manual" />
      </RadioGroup>

      {renderSplitInputs()}

      <Button
        variant="contained"
        fullWidth
        onClick={handleSubmit}
        disabled={amount <= 0}
      >
        Submit
      </Button>
    </Box>
  );
}
