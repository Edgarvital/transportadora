import React from 'react';
import { AuthProvider } from './contexts/AuthContext';
import AppRoutes from './routes';
import './index.css';

const App: React.FC = () => (
  <AuthProvider>
    <AppRoutes />
  </AuthProvider>
);

export default App;
