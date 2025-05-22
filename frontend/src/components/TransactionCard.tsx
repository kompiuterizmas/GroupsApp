import React from 'react';
import { Card, CardContent, Typography } from '@mui/material';
import Grid from '@mui/material/Grid';
import { TransactionDto } from '../services/api';

interface TransactionCardProps {
  /** Full transaction object from API (assumed to include description, splitType, amount, date, payerId, groupId) */
  transaction: TransactionDto;
  /** Name of the group where this transaction occurred */
  groupName: string;
  /** Name of the user who paid */
  payerName: string;
  /** Optional: the balance for the *current* user context */
  userBalance?: number;
}

/**
 * A reusable card that displays all the key details of a transaction.
 */
export default function TransactionCard({
  transaction,
  groupName,
  payerName,
  userBalance
}: TransactionCardProps) {
  return (
    <Card>
      <CardContent>
        {/* 0. Transaction description */}
        <Typography variant="subtitle1" gutterBottom>
          {transaction.description}
        </Typography>

        {/* 1. Split type */}
        <Typography variant="body2">
          <strong>Type:</strong> {transaction.splitType}
        </Typography>

        {/* 2. Amount */}
        <Typography variant="body2">
          <strong>Amount:</strong> {transaction.amount.toFixed(2)}
        </Typography>

        {/* 3. Group */}
        <Typography variant="body2">
          <strong>Group:</strong> {groupName}
        </Typography>

        {/* 4. Payer */}
        <Typography variant="body2">
          <strong>Paid by:</strong> {payerName}
        </Typography>

        {/* 5. Optional per-user balance */}
        {userBalance !== undefined && (
          <Typography variant="body2">
            <strong>Your balance:</strong> {userBalance.toFixed(2)}
          </Typography>
        )}

        {/* 6. Date */}
        <Typography variant="body2">
          <strong>Date:</strong>{' '}
          {new Date(transaction.date).toLocaleString()}
        </Typography>
      </CardContent>
    </Card>
  );
}
