import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import authService from '../api/authService';
import { RegisterPayload } from '../types/auth';

const RegisterPage: React.FC = () => {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});
  const [showSuccess, setShowSuccess] = useState(false);

  const [form, setForm] = useState<RegisterPayload>({
    nome: '',
    documento: '',
    endereco: {
      cep: '',
      logradouro: '',
      numero: '',
      complemento: '',
      bairro: '',
      cidade: '',
      uf: '',
    },
    email: '',
    senha: '',
  });

  const handleChange = (path: string, value: string) => {
    if (path.startsWith('endereco.')) {
      const key = path.split('.')[1];
      setForm((f) => ({ ...f, endereco: { ...f.endereco, [key]: value } }));
    } else {
      setForm((f) => ({ ...f, [path]: value } as RegisterPayload));
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setFieldErrors({});
    setLoading(true);
    try {
      await authService.register(form);
      setShowSuccess(true);
      setLoading(false);
    } catch (err: any) {
      const resp = err?.response?.data;
      if (resp?.status === 400 && resp?.errors) {
        const newFieldErrors: Record<string, string> = {};
        const messages: string[] = [];
        Object.entries(resp.errors).forEach(([key, vals]: any) => {
          const msg = Array.isArray(vals) ? vals.join(' ') : String(vals);
          messages.push(msg);
          const parts = key.split('.');
          const normalized = parts.map(p => p.charAt(0).toLowerCase() + p.slice(1)).join('.');
          newFieldErrors[normalized] = msg;
        });
        setFieldErrors(newFieldErrors);
        setError(messages.join(' '));
      } else {
        setError(err?.response?.data?.message || 'Falha ao registrar usuário.');
      }
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-sky-50 via-white to-indigo-50 px-4">
      <div className="max-w-2xl w-full">
        <div className="bg-white/80 backdrop-blur-md rounded-2xl shadow-lg overflow-hidden">
          <div className="p-8">
            <div className="flex items-center gap-3 mb-6">
              <div className="flex items-center justify-center w-12 h-12 rounded-full bg-indigo-600 text-white font-bold text-xl">T</div>
              <div>
                <h1 className="text-2xl font-semibold">Criar conta</h1>
                <p className="text-sm text-gray-500">Preencha os dados para registrar uma nova conta</p>
              </div>
            </div>

            {error && (
              <div className="mb-4 rounded-md bg-red-50 border border-red-100 p-3 text-red-700">{error}</div>
            )}

            <form onSubmit={handleSubmit} className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div className="col-span-2 md:col-span-1">
                <label className="block text-sm font-medium text-gray-700">Nome</label>
                <input value={form.nome} onChange={(e) => handleChange('nome', e.target.value)} className="mt-1 block w-full rounded-md border border-gray-200 px-4 py-2" required />
                {fieldErrors['nome'] && <p className="mt-1 text-sm text-red-600">{fieldErrors['nome']}</p>}
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">Documento</label>
                <input value={form.documento} onChange={(e) => handleChange('documento', e.target.value)} className="mt-1 block w-full rounded-md border border-gray-200 px-4 py-2" required />
                {fieldErrors['documento'] && <p className="mt-1 text-sm text-red-600">{fieldErrors['documento']}</p>}
              </div>

              <div className="col-span-2">
                <label className="block text-sm font-medium text-gray-700">E-mail</label>
                <input type="email" value={form.email} onChange={(e) => handleChange('email', e.target.value)} className="mt-1 block w-full rounded-md border border-gray-200 px-4 py-2" required />
                {fieldErrors['email'] && <p className="mt-1 text-sm text-red-600">{fieldErrors['email']}</p>}
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">Senha</label>
                <input type="password" value={form.senha} onChange={(e) => handleChange('senha', e.target.value)} className="mt-1 block w-full rounded-md border border-gray-200 px-4 py-2" required />
                {fieldErrors['senha'] && <p className="mt-1 text-sm text-red-600">{fieldErrors['senha']}</p>}
              </div>

              <div className="col-span-2">
                <h2 className="text-lg font-medium mt-4">Endereço</h2>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">CEP</label>
                <input value={form.endereco.cep} onChange={(e) => handleChange('endereco.cep', e.target.value)} className="mt-1 block w-full rounded-md border border-gray-200 px-4 py-2" required />
                {fieldErrors['endereco.cep'] && <p className="mt-1 text-sm text-red-600">{fieldErrors['endereco.cep']}</p>}
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">Logradouro</label>
                <input value={form.endereco.logradouro} onChange={(e) => handleChange('endereco.logradouro', e.target.value)} className="mt-1 block w-full rounded-md border border-gray-200 px-4 py-2" required />
                {fieldErrors['endereco.logradouro'] && <p className="mt-1 text-sm text-red-600">{fieldErrors['endereco.logradouro']}</p>}
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">Número</label>
                <input value={form.endereco.numero} onChange={(e) => handleChange('endereco.numero', e.target.value)} className="mt-1 block w-full rounded-md border border-gray-200 px-4 py-2" required />
                {fieldErrors['endereco.numero'] && <p className="mt-1 text-sm text-red-600">{fieldErrors['endereco.numero']}</p>}
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">Complemento</label>
                <input value={form.endereco.complemento} onChange={(e) => handleChange('endereco.complemento', e.target.value)} className="mt-1 block w-full rounded-md border border-gray-200 px-4 py-2" />
                {fieldErrors['endereco.complemento'] && <p className="mt-1 text-sm text-red-600">{fieldErrors['endereco.complemento']}</p>}
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">Bairro</label>
                <input value={form.endereco.bairro} onChange={(e) => handleChange('endereco.bairro', e.target.value)} className="mt-1 block w-full rounded-md border border-gray-200 px-4 py-2" required />
                {fieldErrors['endereco.bairro'] && <p className="mt-1 text-sm text-red-600">{fieldErrors['endereco.bairro']}</p>}
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">Cidade</label>
                <input value={form.endereco.cidade} onChange={(e) => handleChange('endereco.cidade', e.target.value)} className="mt-1 block w-full rounded-md border border-gray-200 px-4 py-2" required />
                {fieldErrors['endereco.cidade'] && <p className="mt-1 text-sm text-red-600">{fieldErrors['endereco.cidade']}</p>}
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">UF</label>
                <input value={form.endereco.uf} onChange={(e) => handleChange('endereco.uf', e.target.value)} className="mt-1 block w-full rounded-md border border-gray-200 px-4 py-2" required />
                {fieldErrors['endereco.uf'] && <p className="mt-1 text-sm text-red-600">{fieldErrors['endereco.uf']}</p>}
              </div>

              <div className="col-span-2">
                <div className="flex items-center justify-between">
                  <button type="button" onClick={() => navigate('/login')} className="px-4 py-2 rounded-md text-sm border border-gray-200">Voltar ao login</button>
                  <button type="submit" disabled={loading} className="px-6 py-2 bg-gradient-to-r from-indigo-600 to-blue-500 text-white rounded-md">{loading ? 'Registrando...' : 'Registrar'}</button>
                </div>
              </div>
            </form>
          </div>
        </div>
        <p className="mt-4 text-center text-xs text-gray-500">© {new Date().getFullYear()} Transportadora</p>
      </div>

      {showSuccess && (
        <div className="fixed inset-0 z-50 flex items-center justify-center">
          <div className="absolute inset-0 bg-black/40" onClick={() => setShowSuccess(false)} />
          <div className="relative bg-white rounded-lg shadow-lg p-6 w-full max-w-sm z-10">
            <div className="flex flex-col items-center text-center">
              <div className="w-16 h-16 rounded-full bg-green-100 flex items-center justify-center mb-4">
                <svg xmlns="http://www.w3.org/2000/svg" className="w-8 h-8 text-green-600" viewBox="0 0 20 20" fill="currentColor">
                  <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clipRule="evenodd" />
                </svg>
              </div>
              <h3 className="text-lg font-semibold mb-2">Registro bem-sucedido</h3>
              <p className="text-sm text-gray-600 mb-4">Sua conta foi criada com sucesso. Você pode agora entrar.</p>
              <div className="flex gap-3">
                <button onClick={() => { setShowSuccess(false); navigate('/login'); }} className="px-4 py-2 bg-indigo-600 text-white rounded-md">Ir para Login</button>
                <button onClick={() => setShowSuccess(false)} className="px-4 py-2 border rounded-md">Fechar</button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default RegisterPage;
