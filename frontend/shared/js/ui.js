// VitaCare - Componentes de interface reutilizáveis

/* ===== Toasts ===== */
function areaToasts() {
  let area = document.querySelector('.toasts');
  if (!area) {
    area = document.createElement('div');
    area.className = 'toasts';
    area.setAttribute('role', 'status');
    area.setAttribute('aria-live', 'polite');
    document.body.append(area);
  }
  return area;
}

// tipo: 'sucesso' | 'erro' | 'aviso' | 'info'
export function toast(mensagem, tipo = 'info', duracao = 4000) {
  const el = document.createElement('div');
  el.className = `toast toast-${tipo}`;
  el.textContent = mensagem;
  areaToasts().append(el);

  const fechar = () => {
    el.classList.add('saindo');
    el.addEventListener('animationend', () => el.remove(), { once: true });
  };
  el.addEventListener('click', fechar);
  setTimeout(fechar, duracao);
}

/* ===== Modal de confirmação ===== */
export function confirmar({
  titulo,
  mensagem,
  confirmarTexto = 'Confirmar',
  cancelarTexto = 'Voltar',
  perigo = false,
}) {
  return new Promise((resolver) => {
    const dialogo = document.createElement('dialog');
    dialogo.className = 'modal';

    const h = document.createElement('h3');
    h.textContent = titulo;
    const p = document.createElement('p');
    p.className = 'muted';
    p.textContent = mensagem;

    const btnCancelar = document.createElement('button');
    btnCancelar.type = 'button';
    btnCancelar.className = 'btn btn-secondary';
    btnCancelar.textContent = cancelarTexto;

    const btnConfirmar = document.createElement('button');
    btnConfirmar.type = 'button';
    btnConfirmar.className = perigo ? 'btn btn-danger' : 'btn btn-primary';
    btnConfirmar.textContent = confirmarTexto;

    const acoes = document.createElement('div');
    acoes.className = 'modal-acoes';
    acoes.append(btnCancelar, btnConfirmar);
    dialogo.append(h, p, acoes);

    btnConfirmar.addEventListener('click', () => dialogo.close('sim'));
    btnCancelar.addEventListener('click', () => dialogo.close('nao'));
    // Clique fora da caixa fecha; a tecla Esc já fecha sozinha
    dialogo.addEventListener('click', (e) => {
      const r = dialogo.getBoundingClientRect();
      const fora = e.clientX < r.left || e.clientX > r.right || e.clientY < r.top || e.clientY > r.bottom;
      if (fora) dialogo.close('nao');
    });
    dialogo.addEventListener('close', () => {
      dialogo.remove();
      resolver(dialogo.returnValue === 'sim');
    });

    document.body.append(dialogo);
    dialogo.showModal();
    btnCancelar.focus();
  });
}

/* ===== Botão com carregamento ===== */
export async function comCarregamento(botao, tarefa) {
  if (botao.classList.contains('is-loading')) return;
  botao.classList.add('is-loading');
  botao.disabled = true;
  botao.setAttribute('aria-busy', 'true');
  try {
    return await tarefa();
  } finally {
    botao.classList.remove('is-loading');
    botao.disabled = false;
    botao.removeAttribute('aria-busy');
  }
}

/* ===== Efeito de clique nos botões ===== */
document.addEventListener('pointerdown', (e) => {
  const botao = e.target.closest('.btn');
  if (!botao || botao.disabled) return;
  const r = botao.getBoundingClientRect();
  const tam = Math.max(r.width, r.height) * 2;
  const onda = document.createElement('span');
  onda.className = 'ripple';
  onda.style.cssText = `width:${tam}px;height:${tam}px;left:${e.clientX - r.left - tam / 2}px;top:${e.clientY - r.top - tam / 2}px`;
  botao.append(onda);
  onda.addEventListener('animationend', () => onda.remove());
});

/* ===== Tema claro/escuro ===== */
const CHAVE_TEMA = 'vitacare.tema';

export function aplicarTemaSalvo() {
  const salvo = localStorage.getItem(CHAVE_TEMA);
  const escuro = salvo ? salvo === 'escuro' : matchMedia('(prefers-color-scheme: dark)').matches;
  document.documentElement.dataset.theme = escuro ? 'dark' : 'light';
}

export function alternarTema() {
  const escuro = document.documentElement.dataset.theme === 'dark';
  document.documentElement.dataset.theme = escuro ? 'light' : 'dark';
  localStorage.setItem(CHAVE_TEMA, escuro ? 'claro' : 'escuro');
}

aplicarTemaSalvo();

/* ===== Utilidades ===== */
export const formatarDataHora = (iso) =>
  new Date(iso).toLocaleString('pt-BR', {
    weekday: 'long', day: '2-digit', month: 'long', hour: '2-digit', minute: '2-digit',
  });

export const formatarHora = (iso) =>
  new Date(iso).toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' });

export const renderizarIcones = () => window.lucide?.createIcons();