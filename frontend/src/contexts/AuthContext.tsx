import React, { createContext, useState, useEffect, ReactNode } from 'react';
import api from '../api/axiosConfig';
import { User as UserType } from '../types/auth';
import { LoginCredentials, AuthResponse } from '../types/auth';
import { jwtDecode } from 'jwt-decode'; // Importação nomeada correta

type AuthContextType = {
  user: UserType | null;
  isAuthenticated: boolean;
  login: (credentials: LoginCredentials) => Promise<void>;
  logout: () => void;
};

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  
  const [user, setUser] = useState<UserType | null>(() => {
    try {
      const raw = localStorage.getItem('@Transportadora:user');
      if (raw && raw !== "undefined") {
        return JSON.parse(raw) as UserType;
      }
    } catch (error) {
      console.error("Erro ao processar dados do usuário:", error);
      localStorage.removeItem('@Transportadora:user');
    }
    return null;
  });

  const [isAuthenticated, setIsAuthenticated] = useState<boolean>(() => !!localStorage.getItem('@Transportadora:token'));

  useEffect(() => {
    const token = localStorage.getItem('@Transportadora:token');
    if (token) {
      api.defaults.headers.common.Authorization = `Bearer ${token}`;
      setIsAuthenticated(true);
      
      if (!user) {
        try {
          // CORREÇÃO: Chamada correta utilizando camelCase padrão da nova versão da lib
          const payload: any = jwtDecode(token);
          const id = payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] || payload.sub;
          const nome = payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] || payload.name || '';
          const email = payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] || payload.email || '';
          const role = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || payload.role;
          
          const derived: UserType = { id: String(id || ''), nome, email, role };
          setUser(derived);
          localStorage.setItem('@Transportadora:user', JSON.stringify(derived));
        } catch (e) {
          console.warn('Could not decode token to derive user', e);
        }
      }
    } else {
      delete api.defaults.headers.common.Authorization;
      setIsAuthenticated(false);
    }
  }, [user]);

  const login = async (credentials: LoginCredentials) => {
    const response = await api.post<AuthResponse>('/auth/login', credentials);
    const { token, user: returnedUser } = response.data;
    
    localStorage.setItem('@Transportadora:token', token);
    api.defaults.headers.common.Authorization = `Bearer ${token}`;
    
    let finalUser: UserType | null = null;
    
    if (returnedUser) {
      finalUser = returnedUser;
    } else {
      try {
        // CORREÇÃO: Chamada correta utilizando camelCase padrão da nova versão da lib
        const payload: any = jwtDecode(token);
        const id = payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] || payload.sub;
        const nome = payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] || payload.name || '';
        const email = payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] || payload.email || '';
        const role = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || payload.role;
        
        finalUser = { id: String(id || ''), nome, email, role };
      } catch (e) {
        finalUser = null;
      }
    }
    
    if (finalUser) {
      localStorage.setItem('@Transportadora:user', JSON.stringify(finalUser));
      setUser(finalUser);
    }
    setIsAuthenticated(true);
  };

  const logout = () => {
    localStorage.removeItem('@Transportadora:token');
    localStorage.removeItem('@Transportadora:user');
    setUser(null);
    setIsAuthenticated(false);
    delete api.defaults.headers.common.Authorization;
  };

  return (
    <AuthContext.Provider value={{ user, isAuthenticated, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const ctx = React.useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used within AuthProvider');
  return ctx;
};

export default AuthContext;