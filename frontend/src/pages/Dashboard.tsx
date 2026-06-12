import React, { useState, useEffect, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { Plus, Calendar, Filter } from 'lucide-react';
import {
  Solicitacao,
  PaginatedResponse,
  StatusColeta,
  PrioridadeNivel,
} from '../types/solicitacao';
import api from '../api/axiosConfig';

// Estado inicial vazio — será preenchido pela API

const Dashboard: React.FC = () => {
  const navigate = useNavigate();
  const [solicitacoes, setSolicitacoes] = useState<PaginatedResponse<Solicitacao> | null>(null);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);
  const [currentPage, setCurrentPage] = useState<number>(1);

  // Estados de filtro
  const [filterStatus, setFilterStatus] = useState<string>('');
  const [filterPrioridade, setFilterPrioridade] = useState<string>('');
  const [remetenteDocumento, setRemetenteDocumento] = useState<string>('');
  const [destinatarioDocumento, setDestinatarioDocumento] = useState<string>('');
  const [remetenteId, setRemetenteId] = useState<number | null>(null);
  const [destinatarioId, setDestinatarioId] = useState<number | null>(null);
  const [remetenteNomeExibicao, setRemetenteNomeExibicao] = useState<string | null>(null);
  const [destinatarioNomeExibicao, setDestinatarioNomeExibicao] = useState<string | null>(null);
  const [filterDataSolicitacaoInicio, setFilterDataSolicitacaoInicio] = useState<string>('');
  const [filterDataSolicitacaoFim, setFilterDataSolicitacaoFim] = useState<string>('');
  const [filterDataPrevistaInicio, setFilterDataPrevistaInicio] = useState<string>('');
  const [filterDataPrevistaFim, setFilterDataPrevistaFim] = useState<string>('');

  // Estado do modal de roteirização
  const [roteirizarModalOpen, setRoteirizarModalOpen] = useState(false);
  const [roteirizarSolicitacaoId, setRoteirizarSolicitacaoId] = useState<number | null>(null);
  const [selectedMotorista, setSelectedMotorista] = useState<string>('');
  const [selectedVeiculo, setSelectedVeiculo] = useState<string>('');
  const [roteirizarLoading, setRoteirizarLoading] = useState<boolean>(false);
  const [roteirizarError, setRoteirizarError] = useState<string | null>(null);
  const [advancingId, setAdvancingId] = useState<number | null>(null);

  const [motoristas, setMotoristas] = useState<Array<{ id: number; nome: string }>>([]);
  const [veiculos, setVeiculos] = useState<Array<{ id: number; modelo: string; placa: string }>>([]);

  useEffect(() => {
    const fetchLists = async () => {
      try {
        const [mResp, vResp] = await Promise.all([api.get('/motoristas'), api.get('/carros')]);
        setMotoristas(mResp.data || []);
        setVeiculos(vResp.data || []);
      } catch (err) {
        console.error('Erro ao carregar motoristas/veículos', err);
      }
    };

    fetchLists();
  }, []);



  const getStatusBadge = (status: StatusColeta) => {
    const badges = {
      [StatusColeta.Aberta]: 'bg-yellow-100 text-yellow-800 border-yellow-200',
      [StatusColeta.Roteirizada]: 'bg-blue-100 text-blue-800 border-blue-200',
      [StatusColeta.EmColeta]: 'bg-purple-100 text-purple-800 border-purple-200',
      [StatusColeta.Concluida]: 'bg-green-100 text-green-800 border-green-200',
      [StatusColeta.Cancelada]: 'bg-red-100 text-red-800 border-red-200',
    };

    const labels = {
      [StatusColeta.Aberta]: 'Aberta',
      [StatusColeta.Roteirizada]: 'Roteirizada',
      [StatusColeta.EmColeta]: 'Em Coleta',
      [StatusColeta.Concluida]: 'Concluída',
      [StatusColeta.Cancelada]: 'Cancelada',
    };

    return (
      <span className={`px-2 py-1 text-xs font-semibold rounded border ${badges[status]}`}>
        {labels[status]}
      </span>
    );
  };

  const openRoteirizarModal = (id: number) => {
    setRoteirizarSolicitacaoId(id);
    setSelectedMotorista('');
    setSelectedVeiculo('');
    setRoteirizarModalOpen(true);
  };

  const closeRoteirizarModal = () => {
    setRoteirizarModalOpen(false);
    setRoteirizarSolicitacaoId(null);
  };

  const handleConfirmRoteirizacao = async (motoristaId: string, veiculoId: string) => {
    if (!roteirizarSolicitacaoId) return;
    setRoteirizarLoading(true);
    setRoteirizarError(null);
    try {
      console.log('Chamando roteirizar', { solicitacaoId: roteirizarSolicitacaoId, motoristaId, veiculoId });
      await api.put(`/solicitacoes/${roteirizarSolicitacaoId}/roteirizar`, { motoristaId, veiculoId });
      // Atualiza lista
      await fetchSolicitacoes();
      closeRoteirizarModal();
    } catch (err: any) {
      console.error('Erro ao roteirizar', err);
      setRoteirizarError(err?.response?.data?.message || 'Erro ao roteirizar');
    } finally {
      setRoteirizarLoading(false);
    }
  };

  const advanceStatus = async (solicitacaoId: number, currentStatus: StatusColeta) => {
    // Determine next logical status: Roteirizada(2)->EmColeta(3), EmColeta(3)->Concluida(4)
    let nextStatus: StatusColeta | null = null;
    if (currentStatus === StatusColeta.Roteirizada) nextStatus = StatusColeta.EmColeta;
    if (currentStatus === StatusColeta.EmColeta) nextStatus = StatusColeta.Concluida;
    if (!nextStatus) return;

    setAdvancingId(solicitacaoId);
    try {
      await api.patch(`/solicitacoes/${solicitacaoId}/status`, { novoStatus: nextStatus });
      await fetchSolicitacoes();
    } catch (err) {
      console.error('Erro ao avançar status', err);
    } finally {
      setAdvancingId(null);
    }
  };

  const getPrioridadeBadge = (prioridade: PrioridadeNivel) => {
    const badges = {
      [PrioridadeNivel.Baixa]: 'bg-slate-100 text-slate-700 border-slate-200',
      [PrioridadeNivel.Normal]: 'bg-blue-100 text-blue-700 border-blue-200',
      [PrioridadeNivel.Alta]: 'bg-red-100 text-red-700 border-red-300 ring-2 ring-red-400',
    };

    const labels = {
      [PrioridadeNivel.Baixa]: 'Baixa',
      [PrioridadeNivel.Normal]: 'Normal',
      [PrioridadeNivel.Alta]: 'ALTA',
    };

    return (
      <span className={`px-2 py-1 text-xs font-bold rounded border ${badges[prioridade]}`}>
        {labels[prioridade]}
      </span>
    );
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
    });
  };

  const fetchSolicitacoes = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const params: any = {
        PageNumber: currentPage,
        PageSize: 10,
      };

      if (filterStatus) params.Status = parseInt(filterStatus, 10);
      if (filterPrioridade) params.Prioridade = parseInt(filterPrioridade, 10);
      if (remetenteId != null) params.RemetenteId = remetenteId;
      if (destinatarioId != null) params.DestinatarioId = destinatarioId;
      if (filterDataSolicitacaoInicio) params.DataSolicitacaoInicial = new Date(filterDataSolicitacaoInicio + 'T00:00:00').toISOString();
      if (filterDataSolicitacaoFim) params.DataSolicitacaoFinal = new Date(filterDataSolicitacaoFim + 'T23:59:59').toISOString();
      if (filterDataPrevistaInicio) params.DataPrevistaRetiradaInicial = new Date(filterDataPrevistaInicio + 'T00:00:00').toISOString();
      if (filterDataPrevistaFim) params.DataPrevistaRetiradaFinal = new Date(filterDataPrevistaFim + 'T23:59:59').toISOString();

      console.debug('fetchSolicitacoes params', params);
      const resp = await api.get<PaginatedResponse<Solicitacao>>('/solicitacoes', { params });
      setSolicitacoes(resp.data);
    } catch (err: any) {
      console.error('Erro ao buscar solicitações', err);
      setError(err?.response?.data?.message || err.message || 'Erro ao buscar solicitações');
    } finally {
      setLoading(false);
    }
  }, [currentPage, filterStatus, filterPrioridade, remetenteId, destinatarioId, filterDataSolicitacaoInicio, filterDataSolicitacaoFim, filterDataPrevistaInicio, filterDataPrevistaFim]);

  useEffect(() => {
    fetchSolicitacoes();
  }, [fetchSolicitacoes]);

  const handleCheckDocumento = async (documento: string, forWhom: 'remetente' | 'destinatario') => {
    const docClean = documento.replace(/\D/g, '');
    // Se o campo foi limpo, devemos resetar o filtro correspondente
    if (!docClean) {
      if (forWhom === 'remetente') {
        setRemetenteId(null);
        setRemetenteNomeExibicao(null);
      } else {
        setDestinatarioId(null);
        setDestinatarioNomeExibicao(null);
      }
      setError(null);
      return;
    }

    if (docClean.length < 11) {
      // Documento incompleto: remover filtro e avisar
      if (forWhom === 'remetente') {
        setRemetenteId(null);
        setRemetenteNomeExibicao(null);
      } else {
        setDestinatarioId(null);
        setDestinatarioNomeExibicao(null);
      }
      setError('Documento inválido');
      return;
    }
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
        // usuário não encontrado
        if (forWhom === 'remetente') {
          setRemetenteId(null);
          setRemetenteNomeExibicao(null);
        } else {
          setDestinatarioId(null);
          setDestinatarioNomeExibicao(null);
        }
        setError(`Usuário não encontrado para o documento ${docClean}`);
      }
    } catch (err: any) {
      if (err?.response?.status === 404) {
        if (forWhom === 'remetente') {
          setRemetenteId(null);
          setRemetenteNomeExibicao(null);
        } else {
          setDestinatarioId(null);
          setDestinatarioNomeExibicao(null);
        }
        setError(`Usuário não encontrado para o documento ${docClean}`);
      } else {
        console.error('Erro ao consultar documento', err);
        setError('Erro ao consultar documento');
      }
    }
  };

  const clearFilters = () => {
    setFilterStatus('');
    setFilterPrioridade('');
    setRemetenteDocumento('');
    setDestinatarioDocumento('');
    setRemetenteId(null);
    setDestinatarioId(null);
    setRemetenteNomeExibicao(null);
    setDestinatarioNomeExibicao(null);
    setFilterDataSolicitacaoInicio('');
    setFilterDataSolicitacaoFim('');
    setFilterDataPrevistaInicio('');
    setFilterDataPrevistaFim('');
    setError(null);
    setCurrentPage(1);
  };

  return (
    <div className="p-6 space-y-6">
      {/* Cabeçalho */}
      <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
        <div>
          <h1 className="text-3xl font-bold text-slate-900">Gestão de Coletas</h1>
          <p className="text-slate-600 mt-1">Monitore e gerencie todas as solicitações de coleta</p>
        </div>
        <button
          onClick={() => navigate('/nova-solicitacao')}
          className="flex items-center gap-2 px-6 py-3 bg-blue-600 text-white font-semibold rounded-lg hover:bg-blue-700 transition-colors shadow-md"
        >
          <Plus className="w-5 h-5" />
          Nova Coleta
        </button>
      </div>

      {/* Filtros */}
      <div className="bg-white rounded-lg shadow-sm border border-slate-200 p-6">
        <div className="flex items-center justify-between mb-4">
          <div className="flex items-center gap-2">
            <Filter className="w-5 h-5 text-slate-600" />
            <h2 className="text-lg font-semibold text-slate-900">Filtros</h2>
          </div>
          <button
            onClick={clearFilters}
            className="px-3 py-2 text-sm font-medium text-slate-700 bg-white border border-slate-300 rounded-md hover:bg-slate-50"
          >
            Limpar filtros
          </button>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
          <div>
            <label className="block text-sm font-medium text-slate-700 mb-1">Status</label>
            <select
              value={filterStatus}
              onChange={(e) => setFilterStatus(e.target.value)}
              className="w-full px-3 py-2 border border-slate-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              <option value="">Todos</option>
              <option value="1">Aberta</option>
              <option value="2">Roteirizada</option>
              <option value="3">Em Coleta</option>
              <option value="4">Concluída</option>
              <option value="5">Cancelada</option>
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium text-slate-700 mb-1">Prioridade</label>
            <select
              value={filterPrioridade}
              onChange={(e) => setFilterPrioridade(e.target.value)}
              className="w-full px-3 py-2 border border-slate-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              <option value="">Todas</option>
              <option value="1">Baixa</option>
              <option value="2">Normal</option>
              <option value="3">Alta</option>
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium text-slate-700 mb-1">CPF do Remetente</label>
            <div className="relative">
              <input
                type="text"
                inputMode="numeric"
                placeholder="Só números"
                value={remetenteDocumento}
                onChange={(e) => setRemetenteDocumento(e.target.value)}
                onBlur={() => handleCheckDocumento(remetenteDocumento, 'remetente')}
                className="w-full px-3 py-2 border border-slate-300 rounded-md"
              />
            </div>
            {remetenteNomeExibicao && <p className="mt-1 text-sm text-slate-700">{remetenteNomeExibicao}</p>}
          </div>

          <div>
            <label className="block text-sm font-medium text-slate-700 mb-1">CPF do Destinatário</label>
            <div className="relative">
              <input
                type="text"
                inputMode="numeric"
                placeholder="Só números"
                value={destinatarioDocumento}
                onChange={(e) => setDestinatarioDocumento(e.target.value)}
                onBlur={() => handleCheckDocumento(destinatarioDocumento, 'destinatario')}
                className="w-full px-3 py-2 border border-slate-300 rounded-md"
              />
            </div>
            {destinatarioNomeExibicao && <p className="mt-1 text-sm text-slate-700">{destinatarioNomeExibicao}</p>}
          </div>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 mt-4">
          <div>
            <label className="block text-sm font-medium text-slate-700 mb-1">Data Solicitação Início</label>
            <div className="relative">
              <Calendar className="absolute left-3 top-1/2 transform -translate-y-1/2 w-4 h-4 text-slate-400" />
              <input
                type="date"
                value={filterDataSolicitacaoInicio}
                onChange={(e) => setFilterDataSolicitacaoInicio(e.target.value)}
                className="w-full pl-10 pr-3 py-2 border border-slate-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-slate-700 mb-1">Data Solicitação Fim</label>
            <div className="relative">
              <Calendar className="absolute left-3 top-1/2 transform -translate-y-1/2 w-4 h-4 text-slate-400" />
              <input
                type="date"
                value={filterDataSolicitacaoFim}
                onChange={(e) => setFilterDataSolicitacaoFim(e.target.value)}
                className="w-full pl-10 pr-3 py-2 border border-slate-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-slate-700 mb-1">Data Prevista Início</label>
            <div className="relative">
              <Calendar className="absolute left-3 top-1/2 transform -translate-y-1/2 w-4 h-4 text-slate-400" />
              <input
                type="date"
                value={filterDataPrevistaInicio}
                onChange={(e) => setFilterDataPrevistaInicio(e.target.value)}
                className="w-full pl-10 pr-3 py-2 border border-slate-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-slate-700 mb-1">Data Prevista Fim</label>
            <div className="relative">
              <Calendar className="absolute left-3 top-1/2 transform -translate-y-1/2 w-4 h-4 text-slate-400" />
              <input
                type="date"
                value={filterDataPrevistaFim}
                onChange={(e) => setFilterDataPrevistaFim(e.target.value)}
                className="w-full pl-10 pr-3 py-2 border border-slate-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
          </div>
        </div>
      </div>

      {/* Tabela */}
      <div className="bg-white rounded-lg shadow-sm border border-slate-200 overflow-hidden">
        {error && (
          <div className="p-4 bg-red-50 border border-red-200 text-red-800">
            {error}
          </div>
        )}

        <div className="overflow-x-auto relative">
          {loading ? (
            <div className="w-full flex items-center justify-center p-12">
              <svg className="animate-spin h-8 w-8 text-blue-600" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v4a4 4 0 00-4 4H4z"></path>
              </svg>
            </div>
          ) : (
            <table className="w-full">
            <thead className="bg-slate-50 border-b border-slate-200">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-semibold text-slate-700 uppercase tracking-wider">
                  ID
                </th>
                <th className="px-6 py-3 text-left text-xs font-semibold text-slate-700 uppercase tracking-wider">
                  Remetente
                </th>
                <th className="px-6 py-3 text-left text-xs font-semibold text-slate-700 uppercase tracking-wider">
                  Destinatário
                </th>
                <th className="px-6 py-3 text-left text-xs font-semibold text-slate-700 uppercase tracking-wider">
                  Data Prevista
                </th>
                <th className="px-6 py-3 text-left text-xs font-semibold text-slate-700 uppercase tracking-wider">
                  Status
                </th>
                <th className="px-6 py-3 text-left text-xs font-semibold text-slate-700 uppercase tracking-wider">
                  Prioridade
                </th>
                <th className="px-6 py-3 text-left text-xs font-semibold text-slate-700 uppercase tracking-wider">
                  Ações
                </th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-200">
              {solicitacoes?.items?.length ? (
                solicitacoes.items.map((solicitacao) => (
                  <tr
                    key={solicitacao.id}
                    className={`hover:bg-slate-50 transition-colors ${
                      solicitacao.prioridade === PrioridadeNivel.Alta ? 'bg-red-50/30' : ''
                    }`}
                  >
                    <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-slate-900">
                      #{solicitacao.id}
                    </td>
                    <td className="px-6 py-4 text-sm text-slate-700">
                      {solicitacao.remetenteNome}
                    </td>
                    <td className="px-6 py-4 text-sm text-slate-700">
                      {solicitacao.destinatarioNome}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-slate-700">
                      {formatDate(solicitacao.dataPrevistaRetirada)}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      {getStatusBadge(solicitacao.status)}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      {getPrioridadeBadge(solicitacao.prioridade)}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm">
                      {solicitacao.status === StatusColeta.Aberta ? (
                        <div className="flex items-center gap-2">
                          <button
                            onClick={() => openRoteirizarModal(solicitacao.id)}
                            className="px-3 py-1 bg-blue-600 text-white rounded-md text-sm hover:bg-blue-700"
                          >
                            Roteirizar
                          </button>
                          <button
                            onClick={() => navigate(`/solicitacao/${solicitacao.id}`)}
                            className="text-blue-600 hover:text-blue-800 font-medium"
                          >
                            Ver detalhes
                          </button>
                        </div>
                      ) : (
                        <div className="flex items-center gap-2">
                          {(solicitacao.status === StatusColeta.Roteirizada || solicitacao.status === StatusColeta.EmColeta) && (
                            <button
                              onClick={() => advanceStatus(solicitacao.id, solicitacao.status)}
                              disabled={advancingId === solicitacao.id}
                              className="px-2 py-1 bg-emerald-600 text-white rounded-md text-sm hover:bg-emerald-700 disabled:opacity-50"
                            >
                              {advancingId === solicitacao.id ? 'Processando...' : 'Avançar Status'}
                            </button>
                          )}

                          <button
                            onClick={() => navigate(`/solicitacao/${solicitacao.id}`)}
                            className="text-blue-600 hover:text-blue-800 font-medium"
                          >
                            Ver detalhes
                          </button>
                        </div>
                      )}
                    </td>
                  </tr>
                ))
              ) : (
                <tr>
                  <td colSpan={7} className="px-6 py-12 text-center text-sm text-slate-500">
                    Nenhuma solicitação encontrada
                  </td>
                </tr>
              )}
            </tbody>
          </table>
          )}
        </div>

        

          {/* Paginação */}
        {/* Modal de Roteirização */}
        {roteirizarModalOpen && (
          <div className="fixed inset-0 z-50 flex items-center justify-center">
            <div className="absolute inset-0 bg-black/50 backdrop-blur-sm" onClick={closeRoteirizarModal} />
            <div className="relative bg-white rounded-lg shadow-lg w-full max-w-lg mx-4 z-10 p-6">
              <h3 className="text-lg font-semibold">Roteirizar Coleta #{roteirizarSolicitacaoId}</h3>
              <div className="mt-4 space-y-4">
                <div>
                  <label className="block text-sm font-medium text-slate-700 mb-1">Motorista</label>
                  <select
                    value={selectedMotorista}
                    onChange={(e) => setSelectedMotorista(e.target.value)}
                    className="w-full px-3 py-2 border border-slate-300 rounded-md"
                  >
                    <option value="">Selecione um motorista</option>
                    {motoristas.map((m) => (
                      <option key={m.id} value={String(m.id)}>{m.nome}</option>
                    ))}
                  </select>
                </div>

                <div>
                  <label className="block text-sm font-medium text-slate-700 mb-1">Veículo</label>
                  <select
                    value={selectedVeiculo}
                    onChange={(e) => setSelectedVeiculo(e.target.value)}
                    className="w-full px-3 py-2 border border-slate-300 rounded-md"
                  >
                    <option value="">Selecione um veículo</option>
                    {veiculos.map((v) => (
                      <option key={v.id} value={String(v.id)}>{`${v.modelo} - ${v.placa}`}</option>
                    ))}
                  </select>
                </div>
              </div>
              {roteirizarError && (
                <div className="mt-4 p-3 bg-red-50 text-red-700 border border-red-100 rounded">{roteirizarError}</div>
              )}

              <div className="mt-6 flex justify-end gap-2">
                <button onClick={closeRoteirizarModal} className="px-4 py-2 border rounded-md">Cancelar</button>
                <button
                  onClick={() => handleConfirmRoteirizacao(selectedMotorista, selectedVeiculo)}
                  disabled={!selectedMotorista || !selectedVeiculo || roteirizarLoading}
                  className="px-4 py-2 bg-blue-600 text-white rounded-md disabled:opacity-50 flex items-center gap-2"
                >
                  {roteirizarLoading ? (
                    <svg className="animate-spin h-4 w-4" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                      <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                      <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v4a4 4 0 00-4 4H4z"></path>
                    </svg>
                  ) : null}
                  Confirmar Rota
                </button>
              </div>
            </div>
          </div>
        )}
        <div className="px-6 py-4 border-t border-slate-200 flex items-center justify-between">
          <p className="text-sm text-slate-600">
            Mostrando <span className="font-medium">{solicitacoes?.items.length ?? 0}</span> de{' '}
            <span className="font-medium">{solicitacoes?.totalCount ?? 0}</span> solicitações
          </p>
          <div className="flex gap-2">
            <button
              onClick={() => setCurrentPage((p) => Math.max(1, p - 1))}
              disabled={!solicitacoes?.hasPreviousPage}
              className="px-4 py-2 text-sm font-medium text-slate-700 bg-white border border-slate-300 rounded-md hover:bg-slate-50 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              Anterior
            </button>
            <button
              onClick={() => setCurrentPage((p) => p + 1)}
              disabled={!solicitacoes?.hasNextPage}
              className="px-4 py-2 text-sm font-medium text-slate-700 bg-white border border-slate-300 rounded-md hover:bg-slate-50 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              Próxima
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Dashboard;
