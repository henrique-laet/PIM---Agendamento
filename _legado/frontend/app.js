// ===== DADOS SIMULADOS =====
const consultas = [
    {
        paciente: "João Silva",
        medico: "Dr. Carlos Eduardo",
        especialidade: "Clínico Geral",
        dataHora: "24/05/2025 09:00",
        status: "Confirmado"
    },
    {
        paciente: "Maria Souza",
        medico: "Dra. Ana Paula",
        especialidade: "Cardiologia",
        dataHora: "24/05/2025 10:00",
        status: "Pendente"
    },
    {
        paciente: "Pedro Santos",
        medico: "Dr. Carlos Eduardo",
        especialidade: "Clínico Geral",
        dataHora: "25/05/2025 14:00",
        status: "Confirmado"
    },
    {
        paciente: "Lucia Ferreira",
        medico: "Dra. Ana Paula",
        especialidade: "Cardiologia",
        dataHora: "25/05/2025 15:00",
        status: "Cancelado"
    }
];

// ===== PREENCHE A TABELA =====
function carregarConsultas() {
    const tabela = document.getElementById("tabelaConsultas");
    tabela.innerHTML = "";

    consultas.forEach(c => {
        const badgeClass =
            c.status === "Confirmado" ? "badge-confirmado" :
            c.status === "Pendente" ? "badge-pendente" : "badge-cancelado";

        tabela.innerHTML += `
            <tr>
                <td><i class="bi bi-person-fill text-primary"></i> ${c.paciente}</td>
                <td>${c.medico}</td>
                <td>${c.especialidade}</td>
                <td><i class="bi bi-calendar3"></i> ${c.dataHora}</td>
                <td><span class="${badgeClass}">${c.status}</span></td>
            </tr>
        `;
    });
}

// ===== INICIA =====
document.addEventListener("DOMContentLoaded", carregarConsultas);