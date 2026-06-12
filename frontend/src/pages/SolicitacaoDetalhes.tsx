import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import api from '../api/axiosConfig';
import { Solicitacao, TipoOcorrencia, CreateOcorrenciaDto, StatusColeta } from '../types/solicitacao';

const SolicitacaoDetalhes: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [solicitacao, setSolicitacao] = useState<Solicitacao | null>(null);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  // Ocorrência modal/expand
  const [showOcorrenciaForm, setShowOcorrenciaForm] = useState(false);
  const [ocorrenciaTipo, setOcorrenciaTipo] = useState<TipoOcorrencia | ''>('');
  const [ocorrenciaDescricao, setOcorrenciaDescricao] = useState<string>('');
  const [ocorrenciaLoading, setOcorrenciaLoading] = useState(false);
  const [ocorrenciaError, setOcorrenciaError] = useState<string | null>(null);
  const [cancelLoading, setCancelLoading] = useState(false);

  const fetchSolicitacao = async () => {
    if (!id) return;
    setLoading(true);
    setError(null);
    try {
      const resp = await api.get<Solicitacao>(`/solicitacoes/${id}`);
      setSolicitacao(resp.data);
    } catch (err: any) {
      console.error('Erro ao carregar solicitação', err);
      setError(err?.response?.data?.message || 'Erro ao carregar solicitação');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchSolicitacao();
  }, [id]);

  const handleSubmitOcorrencia = async (e?: React.FormEvent) => {
    e?.preventDefault();
    if (!id) return;
    if (!ocorrenciaTipo || !ocorrenciaDescricao.trim()) {
      setOcorrenciaError('Preencha o tipo e a descrição');
      return;
    }
    setOcorrenciaLoading(true);
    setOcorrenciaError(null);
    try {
      const dto: CreateOcorrenciaDto = { tipo: ocorrenciaTipo as TipoOcorrencia, descricao: ocorrenciaDescricao };
      await api.post(`/solicitacoes/${id}/ocorrencias`, dto);
      setShowOcorrenciaForm(false);
      setOcorrenciaDescricao('');
      setOcorrenciaTipo('');
      await fetchSolicitacao();
    } catch (err: any) {
      console.error('Erro ao criar ocorrência', err);
      setOcorrenciaError(err?.response?.data?.message || 'Erro ao criar ocorrência');
    } finally {
      setOcorrenciaLoading(false);
    }
  };

  // determinar ocorrência mais recente por data (API pode retornar em ordem reversa)
  const ocorrencias = solicitacao?.ocorrencias ?? [];
  const mostRecentOcorrencia = ocorrencias.length
    ? ocorrencias.reduce((a, b) => (new Date(a.dataHora).getTime() > new Date(b.dataHora).getTime() ? a : b))
    : null;

  if (loading) return <div className="p-6">Carregando...</div>;
  if (error) return <div className="p-6 text-red-700">{error}</div>;

  return (
    <div className="p-6">
      <div className="flex items-center justify-between mb-6">
        <button onClick={() => navigate('/')} className="text-sm text-blue-600">&larr; Voltar</button>
        <h1 className="text-2xl font-bold">Detalhes da Solicitação #{solicitacao?.id}</h1>
        <div />
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Left column - informações */}
        <div className="lg:col-span-2 space-y-4">
          <div className="bg-white rounded-lg p-4 shadow-sm border border-slate-200">
            <h2 className="text-lg font-semibold mb-2">Remetente / Destinatário</h2>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4 text-sm text-slate-700">
              <div>
                <div className="font-medium">Remetente</div>
                <div>{solicitacao?.remetenteNome}</div>
              </div>
              <div>
                <div className="font-medium">Destinatário</div>
                <div>{solicitacao?.destinatarioNome}</div>
              </div>
            </div>
          </div>

          <div className="bg-white rounded-lg p-4 shadow-sm border border-slate-200">
            <h2 className="text-lg font-semibold mb-2">Dados da Carga</h2>
            <div className="text-sm text-slate-700 space-y-1">
              <div><span className="font-medium">Descrição:</span> {solicitacao?.carga?.descricaoNome}</div>
              <div><span className="font-medium">Tipo:</span> {solicitacao?.carga?.tipo}</div>
              <div><span className="font-medium">Peso:</span> {solicitacao?.carga?.peso} kg</div>
              <div><span className="font-medium">Dimensões:</span> {solicitacao?.carga?.altura} x {solicitacao?.carga?.largura} x {solicitacao?.carga?.comprimento} cm</div>
            </div>
          </div>

          { (solicitacao?.motoristaNome || solicitacao?.veiculoModelo || solicitacao?.veiculoPlaca) && (
            <div className="bg-white rounded-lg p-4 shadow-sm border border-slate-200">
              <h2 className="text-lg font-semibold mb-2">Roteirização</h2>
              <div className="text-sm text-slate-700 space-y-1">
                <div><span className="font-medium">Motorista:</span> {solicitacao?.motoristaNome}</div>
                <div><span className="font-medium">Veículo:</span> {solicitacao?.veiculoModelo}</div>
                <div><span className="font-medium">Placa:</span> {solicitacao?.veiculoPlaca}</div>
              </div>
            </div>
          )}
        </div>

        {/* Right column - histórico e ações */}
        <div className="space-y-4">
          <div className="flex items-center justify-between">
            <h2 className="text-lg font-semibold">Histórico / Ocorrências</h2>
            {solicitacao && solicitacao.status !== StatusColeta.Cancelada && solicitacao.status !== StatusColeta.Concluida && (
              <button
                onClick={() => setShowOcorrenciaForm((s) => !s)}
                className="px-3 py-1 bg-blue-600 text-white rounded-md text-sm"
              >
                + Nova Ocorrência
              </button>
            )}
          </div>

          {showOcorrenciaForm && solicitacao && solicitacao.status !== StatusColeta.Cancelada && solicitacao.status !== StatusColeta.Concluida && (
            <form onSubmit={handleSubmitOcorrencia} className="bg-white p-4 rounded-md border border-slate-200">
              {ocorrenciaError && <div className="mb-2 text-sm text-red-700">{ocorrenciaError}</div>}
              <div className="mb-2">
                <label className="block text-sm text-slate-700">Tipo</label>
                <select className="w-full px-3 py-2 border rounded" value={String(ocorrenciaTipo)} onChange={(e) => setOcorrenciaTipo(Number(e.target.value) as TipoOcorrencia)}>
                  <option value="">Selecione</option>
                  <option value={1}>Cliente Ausente</option>
                  <option value={2}>Endereço Incorreto</option>
                  <option value={3}>Divergência de Carga</option>
                  <option value={4}>Outros</option>
                </select>
              </div>
              <div className="mb-2">
                <label className="block text-sm text-slate-700">Descrição</label>
                <textarea required value={ocorrenciaDescricao} onChange={(e) => setOcorrenciaDescricao(e.target.value)} className="w-full px-3 py-2 border rounded" rows={4} />
              </div>
                <div className="flex items-center justify-between gap-2">
                  <div>
                    {ocorrenciaDescricao.trim().length > 0 && (
                      <button
                        type="button"
                        onClick={async () => {
                          // cancelar solicitação: cria ocorrência e altera status
                          if (!solicitacao?.id) return;
                          setCancelLoading(true);
                          setOcorrenciaError(null);
                          try {
                            const tipoToUse = ocorrenciaTipo || TipoOcorrencia.Outros;
                            const dto: CreateOcorrenciaDto = { tipo: tipoToUse as TipoOcorrencia, descricao: ocorrenciaDescricao };
                            await api.post(`/solicitacoes/${solicitacao.id}/ocorrencias`, dto);
                            await api.patch(`/solicitacoes/${solicitacao.id}/status`, { novoStatus: StatusColeta.Cancelada });
                            await fetchSolicitacao();
                            setShowOcorrenciaForm(false);
                            setOcorrenciaDescricao('');
                            setOcorrenciaTipo('');
                          } catch (err: any) {
                            console.error('Erro ao cancelar solicitação', err);
                            setOcorrenciaError(err?.response?.data?.message || 'Erro ao cancelar solicitação');
                          } finally {
                            setCancelLoading(false);
                          }
                        }}
                        className="px-3 py-2 bg-red-600 text-white rounded"
                        disabled={cancelLoading}
                      >
                        {cancelLoading ? 'Cancelando...' : 'Cancelar Solicitação'}
                      </button>
                    )}
                  </div>
                  <div className="flex gap-2">
                    <button type="button" onClick={() => setShowOcorrenciaForm(false)} className="px-3 py-2 border rounded">Cancelar</button>
                    <button type="submit" disabled={ocorrenciaLoading} className="px-3 py-2 bg-blue-600 text-white rounded">{ocorrenciaLoading ? 'Enviando...' : 'Salvar'}</button>
                  </div>
                </div>
            </form>
          )}

          <div className="bg-white rounded-md border border-slate-200 p-4">
            <ol className="space-y-4">
              {solicitacao?.ocorrencias?.length ? (
                solicitacao.ocorrencias.map((o) => {
                  const isMostRecent = mostRecentOcorrencia ? o.id === mostRecentOcorrencia.id : false;
                  const dotClass = isMostRecent && solicitacao.status === StatusColeta.Cancelada ? 'bg-red-600' : 'bg-blue-600';
                  return (
                    <li key={o.id} className="flex gap-3">
                      <div className={`w-2 h-2 rounded-full ${dotClass} mt-2`} />
                      <div>
                        <div className="text-sm font-medium">{TipoOcorrencia[o.tipo] || 'Ocorrência'}</div>
                        <div className="text-sm text-slate-700">{o.descricao}</div>
                        <div className="text-xs text-slate-500 mt-1 flex items-center gap-2">
                          <span>{new Date(o.dataHora).toLocaleString()} — {o.usuarioNome}</span>
                          {isMostRecent && solicitacao.status === StatusColeta.Cancelada && (
                            <span className="inline-block text-xs font-semibold bg-red-100 text-red-700 px-2 py-0.5 rounded">Cancelamento registrado aqui</span>
                          )}
                        </div>
                      </div>
                    </li>
                  );
                })
              ) : (
                <div className="text-sm text-slate-500">Sem ocorrências</div>
              )}
            </ol>
          </div>
        </div>
      </div>
    </div>
  );
};

export default SolicitacaoDetalhes;
