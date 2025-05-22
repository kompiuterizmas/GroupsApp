import React from 'react';
import { AppBar, Toolbar, Button, Box } from '@mui/material';
import { Link, useNavigate } from 'react-router-dom';
import { seedDatabase } from '../services/seedApi';

export default function NavBar() {
  const navigate = useNavigate();

  const handleSeed = async () => {
    try {
      await seedDatabase();
      alert('Demo data seeded successfully!');
      navigate(0);  // reload page
    } catch {
      alert('Failed to seed demo data.');
    }
  };

  return (
    <AppBar position="fixed">
      <Toolbar>
        <Box sx={{ flexGrow: 1 }}>
          <Button color="inherit" component={Link} to="/groups">
            Groups
          </Button>
          <Button color="inherit" component={Link} to="/users">
            Users
          </Button>
        </Box>
        <Button color="inherit" onClick={handleSeed}>
          Seed Demo Data
        </Button>
      </Toolbar>
    </AppBar>
  );
}