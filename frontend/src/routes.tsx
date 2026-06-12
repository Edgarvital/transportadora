import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { useAuth } from './contexts/AuthContext';
import Layout from './components/Layout';
import LoginPage from './pages/Login';
import RegisterPage from './pages/Register';
import Dashboard from './pages/Dashboard';
import NovaSolicitacao from './pages/NovaSolicitacao';
import SolicitacaoDetalhes from './pages/SolicitacaoDetalhes';

const ProtectedRoute = ({ children, allowedRoles }: { children: JSX.Element; allowedRoles?: string[] }) => {
  const { isAuthenticated, user } = useAuth();
  if (!isAuthenticated) return <Navigate to="/login" replace />;
  if (allowedRoles && !allowedRoles.includes(user?.role || '')) {
    // If authenticated but not allowed, redirect to the page they can access
    return <Navigate to="/nova-solicitacao" replace />;
  }
  return children;
};

const AppRoutes: React.FC = () => (
  <BrowserRouter>
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />
      <Route
        path="/"
        element={
          <ProtectedRoute allowedRoles={["Admin", "Atendente"]}>
            <Layout>
              <Dashboard />
            </Layout>
          </ProtectedRoute>
        }
      />
      <Route
        path="/nova-solicitacao"
        element={
          <ProtectedRoute>
            <Layout>
              <NovaSolicitacao />
            </Layout>
          </ProtectedRoute>
        }
      />
      <Route
        path="/solicitacao/:id"
        element={
          <ProtectedRoute allowedRoles={["Admin", "Atendente"]}>
            <Layout>
              <SolicitacaoDetalhes />
            </Layout>
          </ProtectedRoute>
        }
      />
    </Routes>
  </BrowserRouter>
);

export default AppRoutes;
