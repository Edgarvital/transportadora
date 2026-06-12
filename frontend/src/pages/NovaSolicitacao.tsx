import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Plus } from 'lucide-react';
import api from '../api/axiosConfig';
import { CreateSolicitacaoDto, PrioridadeNivel } from '../types/solicitacao';

const NovaSolicitacao: React.FC = () => {
  const navigate = useNavigate();
  const [remetenteId, setRemetenteId] = useState<number | null>(null);
  const [destinatarioId, setDestinatarioId] = useState<number | null>(null);
  const [remetenteDocumento, setRemetenteDocumento] = useState<string>('');
  const [destinatarioDocumento, setDestinatarioDocumento] = useState<string>('');
  const [remetenteNomeExibicao, setRemetenteNomeExibicao] = useState<string | null>(null);
  const [destinatarioNomeExibicao, setDestinatarioNomeExibicao] = useState<string | null>(null);
  const [dataPrevistaRetirada, setDataPrevistaRetirada] = useState<string>('');
  const [prioridade, setPrioridade] = useState<PrioridadeNivel>(PrioridadeNivel.Normal);
  const [observacoesGerais, setObservacoesGerais] = useState<string>('');

  const [descricaoNome, setDescricaoNome] = useState<string>('');
  const [tipo, setTipo] = useState<string>('');
  const [peso, setPeso] = useState<number | ''>('');
  const [altura, setAltura] = useState<number | ''>('');
  const [largura, setLargura] = useState<number | ''>('');
  const [comprimento, setComprimento] = useState<number | ''>('');

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  // Modal para criar usuário quando documento não existir
  const [showUserModal, setShowUserModal] = useState(false);
  const [userModalFor, setUserModalFor] = useState<'remetente' | 'destinatario' | null>(null);
  const [modalNome, setModalNome] = useState<string>('');
  const [modalDocumento, setModalDocumento] = useState<string>('');
  const [modalEndereco, setModalEndereco] = useState({
    cep: '',
    logradouro: '',
    numero: '',
    complemento: '',
    bairro: '',
    cidade: '',
    uf: '',
  });

  const validate = (): string | null => {
    if (!remetenteId) return 'Remetente é obrigatório (verifique o CPF)';
    if (!destinatarioId) return 'Destinatário é obrigatório (verifique o CPF)';
    if (!dataPrevistaRetirada) return 'Data prevista é obrigatória';
    if (!descricaoNome) return 'Descrição da carga é obrigatória';
    if (!tipo) return 'Tipo da carga é obrigatório';
    if (!peso || Number(peso) <= 0) return 'Peso deve ser maior que zero';
    return null;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    const v = validate();
    if (v) {
      setError(v);
      return;
    }

    const payload: CreateSolicitacaoDto = {
      remetenteId: Number(remetenteId),
      destinatarioId: Number(destinatarioId),
      dataPrevistaRetirada: new Date(dataPrevistaRetirada).toISOString(),
      prioridade: prioridade,
      observacoesGerais: observacoesGerais || undefined,
      carga: {
        descricaoNome,
        tipo,
        peso: Number(peso),
        altura: Number(altura) || 0.001,
        largura: Number(largura) || 0.001,
        comprimento: Number(comprimento) || 0.001,
      },
    };

    try {
      setLoading(true);
      await api.post('/solicitacoes', payload);
      navigate('/');
    } catch (err: any) {
      console.error(err);
      setError(err?.response?.data?.message || err.message || 'Erro ao criar solicitação');
    } finally {
      setLoading(false);
    }
  };

  const clearUserModal = () => {
    setShowUserModal(false);
    setUserModalFor(null);
    setModalNome('');
    setModalDocumento('');
    setModalEndereco({ cep: '', logradouro: '', numero: '', complemento: '', bairro: '', cidade: '', uf: '' });
  };

  const handleCheckDocumento = async (documento: string, forWhom: 'remetente' | 'destinatario') => {
    const docClean = documento.replace(/\D/g, '');
    if (!docClean || docClean.length < 11) return;
    try {
      const resp = await api.get(`/users/documento/${docClean}`);
      const user = resp.data;
      if (user && user.id) {
        if (forWhom === 'remetente') {
          setRemetenteId(Number(user.id));
          setRemetenteNomeExibicao(user.nome || user.name || null);
        } else {
          setDestinatarioId(Number(user.id));
          setDestinatarioNomeExibicao(user.nome || user.name || null);
        }
      } else {
        // abrir modal para criar usuário
        setUserModalFor(forWhom);
        setModalDocumento(docClean);
        setShowUserModal(true);
      }
    } catch (err: any) {
      if (err?.response?.status === 404) {
        setUserModalFor(forWhom);
        setModalDocumento(docClean);
        setShowUserModal(true);
      } else {
        console.error('Erro ao consultar documento', err);
        setError('Erro ao consultar documento');
      }
    }
  };

  const handleRegisterUserFromModal = async () => {
    setError(null);
    try {
      setLoading(true);
      const payload: any = {
        nome: modalNome,
        documento: modalDocumento,
        endereco: modalEndereco,
      };
      const resp = await api.post('/users/register', payload);
      const created = resp.data;
      if (created && created.id) {
        if (userModalFor === 'remetente') {
          setRemetenteId(Number(created.id));
          setRemetenteNomeExibicao(created.nome || null);
          setRemetenteDocumento(modalDocumento);
        } else if (userModalFor === 'destinatario') {
          setDestinatarioId(Number(created.id));
          setDestinatarioNomeExibicao(created.nome || null);
          setDestinatarioDocumento(modalDocumento);
        }
        clearUserModal();
      } else {
        setError('Erro ao criar usuário');
      }
    } catch (err: any) {
      console.error('Erro ao registrar usuário', err);
      setError(err?.response?.data?.message || err.message || 'Erro ao registrar usuário');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="p-6">
      <div className="flex items-center justify-between mb-6">
        <div>
          <h1 className="text-2xl font-bold">Nova Solicitação</h1>
          <p className="text-slate-600">Crie uma nova solicitação de coleta</p>
        </div>
        <div />
      </div>

      {error && <div className="mb-4 p-3 bg-red-50 text-red-800 border border-red-200">{error}</div>}

      <form onSubmit={handleSubmit} className="space-y-6 bg-white p-6 rounded-lg shadow-sm border border-slate-200">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <label className="block text-sm font-medium text-slate-700 mb-1">CPF do Remetente</label>
            <input
              type="text"
              inputMode="numeric"
              placeholder="Só números"
              value={remetenteDocumento}
              onChange={(e) => setRemetenteDocumento(e.target.value)}
              onBlur={() => handleCheckDocumento(remetenteDocumento, 'remetente')}
              className="w-full px-3 py-2 border rounded-md"
            />
            {remetenteNomeExibicao && <p className="mt-1 text-sm text-slate-700">{remetenteNomeExibicao}</p>}
          </div>

          <div>
            <label className="block text-sm font-medium text-slate-700 mb-1">CPF do Destinatário</label>
            <input
              type="text"
              inputMode="numeric"
              placeholder="Só números"
              value={destinatarioDocumento}
              onChange={(e) => setDestinatarioDocumento(e.target.value)}
              onBlur={() => handleCheckDocumento(destinatarioDocumento, 'destinatario')}
              className="w-full px-3 py-2 border rounded-md"
            />
            {destinatarioNomeExibicao && <p className="mt-1 text-sm text-slate-700">{destinatarioNomeExibicao}</p>}
          </div>

          <div>
            <label className="block text-sm font-medium text-slate-700 mb-1">Data Prevista Retirada</label>
            <input type="datetime-local" value={dataPrevistaRetirada} onChange={(e) => setDataPrevistaRetirada(e.target.value)} className="w-full px-3 py-2 border rounded-md" />
          </div>

          <div>
            <label className="block text-sm font-medium text-slate-700 mb-1">Prioridade</label>
            <select value={prioridade} onChange={(e) => setPrioridade(Number(e.target.value) as PrioridadeNivel)} className="w-full px-3 py-2 border rounded-md">
              <option value={PrioridadeNivel.Baixa}>Baixa</option>
              <option value={PrioridadeNivel.Normal}>Normal</option>
              <option value={PrioridadeNivel.Alta}>Alta</option>
            </select>
          </div>
        </div>

        <div>
          <label className="block text-sm font-medium text-slate-700 mb-1">Observações Gerais</label>
          <textarea value={observacoesGerais} onChange={(e) => setObservacoesGerais(e.target.value)} className="w-full px-3 py-2 border rounded-md" rows={3} />
        </div>

        <div className="border-t pt-4">
          <h3 className="text-lg font-medium mb-3">Dados da Carga</h3>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div>
              <label className="block text-sm font-medium text-slate-700 mb-1">Descrição</label>
              <input value={descricaoNome} onChange={(e) => setDescricaoNome(e.target.value)} className="w-full px-3 py-2 border rounded-md" />
            </div>
            <div>
              <label className="block text-sm font-medium text-slate-700 mb-1">Tipo</label>
              <input value={tipo} onChange={(e) => setTipo(e.target.value)} className="w-full px-3 py-2 border rounded-md" />
            </div>
            <div>
              <label className="block text-sm font-medium text-slate-700 mb-1">Peso (kg)</label>
              <input type="number" step="0.001" value={peso} onChange={(e) => setPeso(e.target.value ? Number(e.target.value) : '')} className="w-full px-3 py-2 border rounded-md" />
            </div>
            <div>
              <label className="block text-sm font-medium text-slate-700 mb-1">Altura (m)</label>
              <input type="number" step="0.001" value={altura} onChange={(e) => setAltura(e.target.value ? Number(e.target.value) : '')} className="w-full px-3 py-2 border rounded-md" />
            </div>
            <div>
              <label className="block text-sm font-medium text-slate-700 mb-1">Largura (m)</label>
              <input type="number" step="0.001" value={largura} onChange={(e) => setLargura(e.target.value ? Number(e.target.value) : '')} className="w-full px-3 py-2 border rounded-md" />
            </div>
            <div>
              <label className="block text-sm font-medium text-slate-700 mb-1">Comprimento (m)</label>
              <input type="number" step="0.001" value={comprimento} onChange={(e) => setComprimento(e.target.value ? Number(e.target.value) : '')} className="w-full px-3 py-2 border rounded-md" />
            </div>
          </div>
        </div>

        <div className="flex items-center justify-end gap-3">
          <button type="button" onClick={() => navigate('/')} className="px-4 py-2 bg-white border rounded-md">Cancelar</button>
          <button type="submit" disabled={loading} className="px-4 py-2 bg-blue-600 text-white rounded-md flex items-center gap-2">
            {loading ? 'Enviando...' : (
              <>
                <Plus className="w-4 h-4" />
                Criar Solicitação
              </>
            )}
          </button>
        </div>
      </form>
      {/* Modal para criação mínima de usuário */}
      {showUserModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center">
          <div className="absolute inset-0 bg-black/50" onClick={clearUserModal} />
          <div className="bg-white rounded-lg shadow-lg z-10 w-full max-w-2xl p-6">
            <h2 className="text-lg font-semibold mb-2">Criar usuário</h2>
            <p className="text-sm text-slate-600 mb-4">Documento não encontrado. Informe dados mínimos para cadastro.</p>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-slate-700 mb-1">Nome</label>
                <input value={modalNome} onChange={(e) => setModalNome(e.target.value)} className="w-full px-3 py-2 border rounded-md" />
              </div>
              <div>
                <label className="block text-sm font-medium text-slate-700 mb-1">Documento</label>
                <input value={modalDocumento} onChange={(e) => setModalDocumento(e.target.value)} className="w-full px-3 py-2 border rounded-md" />
              </div>
              <div>
                <label className="block text-sm font-medium text-slate-700 mb-1">CEP</label>
                <input value={modalEndereco.cep} onChange={(e) => setModalEndereco({ ...modalEndereco, cep: e.target.value })} className="w-full px-3 py-2 border rounded-md" />
              </div>
              <div>
                <label className="block text-sm font-medium text-slate-700 mb-1">Logradouro</label>
                <input value={modalEndereco.logradouro} onChange={(e) => setModalEndereco({ ...modalEndereco, logradouro: e.target.value })} className="w-full px-3 py-2 border rounded-md" />
              </div>
              <div>
                <label className="block text-sm font-medium text-slate-700 mb-1">Número</label>
                <input value={modalEndereco.numero} onChange={(e) => setModalEndereco({ ...modalEndereco, numero: e.target.value })} className="w-full px-3 py-2 border rounded-md" />
              </div>
              <div>
                <label className="block text-sm font-medium text-slate-700 mb-1">Complemento</label>
                <input value={modalEndereco.complemento} onChange={(e) => setModalEndereco({ ...modalEndereco, complemento: e.target.value })} className="w-full px-3 py-2 border rounded-md" />
              </div>
              <div>
                <label className="block text-sm font-medium text-slate-700 mb-1">Bairro</label>
                <input value={modalEndereco.bairro} onChange={(e) => setModalEndereco({ ...modalEndereco, bairro: e.target.value })} className="w-full px-3 py-2 border rounded-md" />
              </div>
              <div>
                <label className="block text-sm font-medium text-slate-700 mb-1">Cidade</label>
                <input value={modalEndereco.cidade} onChange={(e) => setModalEndereco({ ...modalEndereco, cidade: e.target.value })} className="w-full px-3 py-2 border rounded-md" />
              </div>
              <div>
                <label className="block text-sm font-medium text-slate-700 mb-1">UF</label>
                <input value={modalEndereco.uf} onChange={(e) => setModalEndereco({ ...modalEndereco, uf: e.target.value })} className="w-full px-3 py-2 border rounded-md" />
              </div>
            </div>

            <div className="mt-4 flex justify-end gap-3">
              <button type="button" onClick={clearUserModal} className="px-4 py-2 bg-white border rounded-md">Cancelar</button>
              <button type="button" onClick={handleRegisterUserFromModal} className="px-4 py-2 bg-blue-600 text-white rounded-md">Criar usuário</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default NovaSolicitacao;
