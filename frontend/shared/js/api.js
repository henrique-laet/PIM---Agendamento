// VitaCare - Camada única de comunicação com a API
export const API_URL = 'http://localhost:5000/api';
const CHAVE_SESSAO = 'vitacare.sessao';

export const sessao = {
  salvar(dados) { localStorage.setItem(CHAVE_SESSAO, JSON.stringify(dados)); },
  obter() {
    try { return JSON.parse(localStorage.getItem(CHAVE_SESSAO)); } catch { return null; }
  },
  limpar() { localStorage.removeItem(CHAVE_SESSAO); },
  get token() { return this.obter()?.token ?? null; },
  get perfil() { return this.obter()?.perfil ?? null; },
  expirada() {
    const s = this.obter();
    return !s || new Date(s.expiraEm) <= new Date();
  },
};

export class ApiError extends Error {
  constructor(mensagem, status) {
    super(mensagem);
    this.status = status;
  }
}

function extrairMensagem(dados, status) {
  if (dados?.erro) return dados.erro;
  if (dados?.errors) {
    const primeira = Object.values(dados.errors).flat()[0];
    if (primeira) return primeira;
  }
  if (status === 401) return 'Sua sessão expirou. Faça login novamente.';
  if (status === 403) return 'Você não tem permissão para isso.';
  if (status === 404) return 'Não encontrado.';
  return 'Algo deu errado. Tente novamente.';
}

export async function api(caminho, { metodo = 'GET', corpo, autenticado = true } = {}) {
  const cabecalhos = { Accept: 'application/json' };
  if (corpo !== undefined) cabecalhos['Content-Type'] = 'application/json';
  if (autenticado && sessao.token) cabecalhos.Authorization = `Bearer ${sessao.token}`;

  let resposta;
  try {
    resposta = await fetch(`${API_URL}${caminho}`, {
      method: metodo,
      headers: cabecalhos,
      body: corpo !== undefined ? JSON.stringify(corpo) : undefined,
    });
  } catch {
    throw new ApiError('Não foi possível conectar ao servidor. Verifique se a API está rodando.', 0);
  }

  const dados = resposta.status === 204 ? null : await resposta.json().catch(() => null);
  if (!resposta.ok) throw new ApiError(extrairMensagem(dados, resposta.status), resposta.status);
  return dados;
}