import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import {
  getUserById,
  getUserGroups,
  getUserTransactions,
  UserDto,
  GroupDto,
  TransactionDto,
} from "../services/api";
import { Box, Typography, Divider, CircularProgress } from "@mui/material";
import Grid from "@mui/material/Grid";
import TransactionCard from "../components/TransactionCard";

interface TxWithBalance extends TransactionDto {
  balance: number;
}

export default function UserPage() {
  const { userId } = useParams<{ userId: string }>();
  const [user, setUser] = useState<UserDto | null>(null);
  const [groups, setGroups] = useState<GroupDto[]>([]);
  const [transactions, setTransactions] = useState<TxWithBalance[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(false);

  useEffect(() => {
    if (!userId) {
      setError(true);
      setLoading(false);
      return;
    }
    const idNum = parseInt(userId, 10);
    if (isNaN(idNum)) {
      setError(true);
      setLoading(false);
      return;
    }

    async function fetchData() {
      try {
        const u = await getUserById(idNum);
        setUser(u);

        const g = await getUserGroups(idNum);
        setGroups(g);

        const txs = await getUserTransactions(idNum);
        const txsWithBal = txs.map((tx) => {
          const share = tx.splitDetails?.[idNum] ?? 0;
          const bal = tx.payerId === idNum ? tx.amount - share : -share;
          return { ...tx, balance: bal };
        });
        setTransactions(txsWithBal);
      } catch (err) {
        console.error("Failed to load user data", err);
        setError(true);
      } finally {
        setLoading(false);
      }
    }

    fetchData();
  }, [userId]);

  if (loading) {
    return (
      <Box p={4} textAlign="center">
        <CircularProgress />
      </Box>
    );
  }

  if (error || !user) {
    return (
      <Box p={4}>
        <Typography color="error">
          User not found or error loading data.
        </Typography>
      </Box>
    );
  }

  const totalBalance = transactions.reduce((sum, tx) => sum + tx.balance, 0);

  return (
    <Box p={4} sx={{ maxWidth: 800, mx: "auto" }}>
      <Typography variant="h4" gutterBottom>
        {user.name} (ID: {user.id})
      </Typography>

      <Divider sx={{ my: 2 }} />
      <Typography variant="h6">
        Total Balance: {totalBalance.toFixed(2)}
      </Typography>

      <Divider sx={{ my: 2 }} />
      <Typography variant="h6">Groups</Typography>
      {groups.length > 0 ? (
        <ul>
          {groups.map((g) => (
            <li key={g.id}>
              {g.title} (Balance: {g.balance.toFixed(2)})
            </li>
          ))}
        </ul>
      ) : (
        <Typography>No groups</Typography>
      )}

      <Divider sx={{ my: 2 }} />
      <Typography variant="h6">Transactions</Typography>
      {transactions.length > 0 ? (
        <Grid container spacing={2}>
          {transactions.map((tx) => {
            // find the group title
            const group = groups.find((g) => g.id === tx.groupId);
            const groupTitle = group?.title ?? `#${tx.groupId}`;

            // find the payer name
            const payerName =
              tx.payerId === user.id
                ? user.name
                : (() => {
                    // flatten members arrays safely
                    const allMembers = groups.flatMap((g) => g.members ?? []);
                    const member = allMembers.find((m) => m.id === tx.payerId);
                    return member?.name ?? `#${tx.payerId}`;
                  })();

            return (
              <Grid key={tx.id} size={{ xs: 12, sm: 6, md: 4 }}>
                <TransactionCard
                  transaction={tx}
                  groupName={groupTitle}
                  payerName={payerName}
                  userBalance={tx.balance}
                />
              </Grid>
            );
          })}
        </Grid>
      ) : (
        <Typography>No transactions</Typography>
      )}
    </Box>
  );
}
