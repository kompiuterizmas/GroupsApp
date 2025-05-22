import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import ConnectionChecker from './components/ConnectionChecker';
import NavBar from './components/NavBar';
import GroupsListPage from './pages/GroupsListPage';
import GroupDetailPage from './pages/GroupDetailPage';
import NewTransactionPage from './pages/NewTransactionPage';
import MembersPage from './pages/MembersPage';
import UserPage from './pages/UserPage';
import { Box, Toolbar } from '@mui/material';

export default function App() {
  return (
    <ConnectionChecker>
      <BrowserRouter>
        <NavBar />
        {/* Spacer to offset fixed NavBar height */}
        <Toolbar />
        <Box
          component="main"
          sx={{ p: 4, display: 'flex', justifyContent: 'center' }}
        >
          <Routes>
            <Route path="/" element={<Navigate to="/groups" replace />} />
            <Route path="/groups" element={<GroupsListPage />} />
            <Route path="/groups/:groupId" element={<GroupDetailPage />} />
            <Route
              path="/groups/:groupId/new-transaction"
              element={<NewTransactionPage />}
            />
            <Route path="/users" element={<MembersPage />} />
            <Route path="/users/:userId" element={<UserPage />} />
          </Routes>
        </Box>
      </BrowserRouter>
    </ConnectionChecker>
  );
}
