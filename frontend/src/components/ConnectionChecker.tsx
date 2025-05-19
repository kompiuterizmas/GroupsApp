import React, { useState, useEffect } from 'react';
import { getGroups } from '../services/api';
import NetworkErrorPage from './NetworkErrorPage';

interface Props { children: React.ReactNode; }

export default function ConnectionChecker({ children }: Props) {
  const [connected, setConnected] = useState<boolean | null>(null);
  const checkConnection = async () => {
    try { await getGroups(); setConnected(true); }
    catch { setConnected(false); }
  };
  useEffect(() => { checkConnection(); }, []);
  if (connected === false) return <NetworkErrorPage onRetry={checkConnection} />;
  if (connected === null) return <div style={{ textAlign: 'center', marginTop: '2rem' }}>Checking server connection...</div>;
  return <>{children}</>;
}
