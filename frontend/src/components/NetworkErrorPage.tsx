import React from 'react';
import { Box, Typography, Button } from '@mui/material';

interface Props {
  onRetry: () => void;
}

export default function NetworkErrorPage({ onRetry }: Props) {
  return (
    <Box textAlign="center" mt={8}>
      <Typography variant="h5" gutterBottom>
        Cannot connect to the server
      </Typography>
      <Typography variant="body1" gutterBottom>
        Please make sure the backend API is running.
      </Typography>
      <Button variant="contained" onClick={onRetry}>
        Retry
      </Button>
    </Box>
  );
}
