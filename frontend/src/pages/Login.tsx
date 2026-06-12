import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { LoginCredentials } from '../types/auth';

const LoginPage: React.FC = () => {
  const { login, user } = useAuth();
  const navigate = useNavigate();

  const [email, setEmail] = useState('');
  const [senha, setSenha] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setFieldErrors({});
    setLoading(true);
    const credentials: LoginCredentials = { email, senha };
    try {
      await login(credentials);
      // Read stored user synchronously to decide redirect
      try {
        const raw = localStorage.getItem('@Transportadora:user');
        const storedUser = raw ? JSON.parse(raw) : null;
        if (storedUser?.role === 'Cliente') navigate('/nova-solicitacao');
        else navigate('/');
      } catch {
        navigate('/');
      }
    } catch (err: any) {
      // Handle validation errors (400) with possible 'errors' structure
      const resp = err?.response?.data;
      if (resp?.status === 400 && resp?.errors) {
        // flatten messages
        const messages: string[] = [];
        const newFieldErrors: Record<string, string> = {};
        Object.entries(resp.errors).forEach(([key, vals]: any) => {
          const msg = Array.isArray(vals) ? vals.join(' ') : String(vals);
          messages.push(msg);
          // normalize key like 'Endereco.UF' -> 'endereco.uf'
          const parts = key.split('.');
          const normalized = parts.map(p => p.charAt(0).toLowerCase() + p.slice(1)).join('.');
          newFieldErrors[normalized] = msg;
        });
        setFieldErrors(newFieldErrors);
        setError(messages.join(' '));
      } else {
        setError(err?.response?.data?.message || 'Falha ao autenticar. Verifique suas credenciais.');
      }
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-slate-100 px-4">
      {/* Container Principal */}
      <div className="max-w-md w-full">
        
        {/* Card de Login */}
        <div className="bg-white rounded-xl shadow-xl border border-slate-200 overflow-hidden">
          
          {/* Cabeçalho do Card (Branding) */}
          <div className="bg-slate-900 p-8 text-center">
            <div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-blue-600 mb-4 shadow-lg text-white">
              {/* Ícone de Caminhão SVG embutido */}
              <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-8 h-8">
                <path strokeLinecap="round" strokeLinejoin="round" d="M8.25 18.75a1.5 1.5 0 0 1-3 0m3 0a1.5 1.5 0 0 0-3 0m3 0h6m-9 0H3.375a1.125 1.125 0 0 1-1.125-1.125V14.25m17.25 4.5a1.5 1.5 0 0 1-3 0m3 0a1.5 1.5 0 0 0-3 0m3 0h1.125c.621 0 1.129-.504 1.09-1.124a17.902 17.902 0 0 0-3.213-9.193 2.056 2.056 0 0 0-1.58-.86H14.25M16.5 18.75h-2.25m0-11.177v-.958c0-.568-.422-1.048-.987-1.106a48.554 48.554 0 0 0-10.026 0 1.106 1.106 0 0 0-.987 1.106v7.635m12-6.677v6.677m0 4.5v-4.5m0 0h-12" />
              </svg>
            </div>
            <h1 className="text-2xl font-bold text-white tracking-wide">Transpo<span className="text-blue-400">Log</span></h1>
            <p className="text-slate-400 mt-1 text-sm">Painel Operacional de Coletas</p>
          </div>

          {/* Área do Formulário */}
          <div className="p-8">
            {error && (
              <div className="mb-6 rounded-md bg-red-50 border border-red-200 p-4 text-sm text-red-700 flex items-start">
                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" className="w-5 h-5 mr-2 mt-0.5 flex-shrink-0">
                  <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.28 7.22a.75.75 0 00-1.06 1.06L8.94 10l-1.72 1.72a.75.75 0 101.06 1.06L10 11.06l1.72 1.72a.75.75 0 101.06-1.06L11.06 10l1.72-1.72a.75.75 0 00-1.06-1.06L10 8.94 8.28 7.22z" clipRule="evenodd" />
                </svg>
                <span>{error}</span>
              </div>
            )}

            <form onSubmit={handleSubmit} className="space-y-5">
              <div>
                <label className="block text-sm font-semibold text-slate-700 mb-1">E-mail corporativo</label>
                <input
                  type="email"
                  placeholder="operacao@transportadora.com"
                  className="block w-full rounded-lg border border-slate-300 bg-slate-50 px-4 py-2.5 text-slate-900 focus:bg-white focus:border-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-500/20 transition-colors"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  required
                />
                  {fieldErrors['email'] && <p className="mt-1 text-sm text-red-600">{fieldErrors['email']}</p>}
              </div>

              <div>
                <div className="flex items-center justify-between mb-1">
                  <label className="block text-sm font-semibold text-slate-700">Senha de acesso</label>
                </div>
                <input
                  type="password"
                  placeholder="••••••••"
                  className="block w-full rounded-lg border border-slate-300 bg-slate-50 px-4 py-2.5 text-slate-900 focus:bg-white focus:border-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-500/20 transition-colors"
                  value={senha}
                  onChange={(e) => setSenha(e.target.value)}
                  required
                />
                  {fieldErrors['senha'] && <p className="mt-1 text-sm text-red-600">{fieldErrors['senha']}</p>}
              </div>

              <div className="flex items-center justify-between text-sm pt-2">
                <label className="inline-flex items-center gap-2 cursor-pointer">
                  <input type="checkbox" className="w-4 h-4 rounded border-slate-300 text-blue-600 focus:ring-blue-500" />
                  <span className="text-slate-600 font-medium">Manter conectado</span>
                </label>
              </div>

              <button
                type="submit"
                disabled={loading}
                className="w-full py-2.5 px-4 mt-2 bg-blue-600 hover:bg-blue-700 text-white font-semibold rounded-lg shadow-sm transition-all focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 disabled:opacity-70 disabled:cursor-not-allowed flex justify-center items-center"
              >
                {loading ? (
                  <>
                    <svg className="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                      <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                      <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                    </svg>
                    Acessando...
                  </>
                ) : (
                  'Entrar no Sistema'
                )}
              </button>
            </form>

            <div className="mt-4 text-center">
              <p className="text-sm text-slate-600">
                Ainda não tem uma conta?{' '}
                <button type="button" onClick={() => navigate('/register')} className="text-blue-600 font-semibold hover:underline">Registre-se</button>
              </p>
            </div>
          </div>
        </div>

        <p className="mt-6 text-center text-sm text-slate-500 font-medium">
          © {new Date().getFullYear()} Sistema de Transporte. Todos os direitos reservados.
        </p>
      </div>
    </div>
  );
};

export default LoginPage;