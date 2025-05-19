import React from 'react';
import { AppBar, Toolbar, Typography, Button } from '@mui/material';
import { Link } from 'react-router-dom';

export default function NavBar() {
  return (
    <AppBar position="fixed">
      <Toolbar>
        <Typography
          variant="h6"
          component={Link}
          to="/groups"
          sx={{ textDecoration: 'none', color: 'inherit', flexGrow: 1 }}
        >
          GroupsApp
        </Typography>
        <Button color="inherit" component={Link} to="/groups">
          Groups
        </Button>
        <Button color="inherit" component={Link} to="/users">
          Users
        </Button>
      </Toolbar>
    </AppBar>
  );
}