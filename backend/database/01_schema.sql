-- VitaCare - Esquema do banco de dados (PostgreSQL)
-- Rode este arquivo uma vez. Ele recria as tabelas do zero.

DROP TABLE IF EXISTS agendamentos CASCADE;
DROP TABLE IF EXISTS horarios_disponiveis CASCADE;
DROP TABLE IF EXISTS medicos CASCADE;
DROP TABLE IF EXISTS especialidades CASCADE;
DROP TABLE IF EXISTS pacientes CASCADE;
DROP TABLE IF EXISTS usuarios CASCADE;

-- Login de todo mundo (paciente e admin)
CREATE TABLE usuarios (
    id          SERIAL PRIMARY KEY,
    nome        VARCHAR(120) NOT NULL,
    email       VARCHAR(160) NOT NULL UNIQUE,
    senha_hash  VARCHAR(255) NOT NULL,
    perfil      VARCHAR(20)  NOT NULL CHECK (perfil IN ('paciente', 'admin')),
    ativo       BOOLEAN      NOT NULL DEFAULT TRUE,
    criado_em   TIMESTAMP    NOT NULL DEFAULT NOW()
);

-- Dados extras que só o paciente tem
CREATE TABLE pacientes (
    id               SERIAL PRIMARY KEY,
    usuario_id       INT NOT NULL UNIQUE REFERENCES usuarios(id) ON DELETE CASCADE,
    cpf              VARCHAR(11) NOT NULL UNIQUE,
    telefone         VARCHAR(20),
    data_nascimento  DATE
);

CREATE TABLE especialidades (
    id     SERIAL PRIMARY KEY,
    nome   VARCHAR(80) NOT NULL UNIQUE,
    icone  VARCHAR(40)
);

CREATE TABLE medicos (
    id                SERIAL PRIMARY KEY,
    nome              VARCHAR(120) NOT NULL,
    crm               VARCHAR(20)  NOT NULL UNIQUE,
    especialidade_id  INT NOT NULL REFERENCES especialidades(id),
    foto_url          VARCHAR(300),
    bio               TEXT,
    ativo             BOOLEAN NOT NULL DEFAULT TRUE
);

-- Cada linha é um horário que o médico oferece
CREATE TABLE horarios_disponiveis (
    id          SERIAL PRIMARY KEY,
    medico_id   INT NOT NULL REFERENCES medicos(id) ON DELETE CASCADE,
    data_hora   TIMESTAMP NOT NULL,
    disponivel  BOOLEAN NOT NULL DEFAULT TRUE,
    UNIQUE (medico_id, data_hora)
);

CREATE TABLE agendamentos (
    id           SERIAL PRIMARY KEY,
    paciente_id  INT NOT NULL REFERENCES pacientes(id),
    medico_id    INT NOT NULL REFERENCES medicos(id),
    horario_id   INT NOT NULL REFERENCES horarios_disponiveis(id),
    status       VARCHAR(20) NOT NULL DEFAULT 'agendada'
                 CHECK (status IN ('agendada', 'cancelada', 'concluida')),
    criado_em    TIMESTAMP NOT NULL DEFAULT NOW()
);

-- Impede dois agendamentos ATIVOS no mesmo horário.
-- Se um for cancelado, o horário pode ser reservado de novo.
CREATE UNIQUE INDEX ux_agendamento_horario_ativo
    ON agendamentos (horario_id) WHERE status = 'agendada';

CREATE INDEX ix_medicos_especialidade ON medicos (especialidade_id);
CREATE INDEX ix_horarios_medico_data  ON horarios_disponiveis (medico_id, data_hora);
CREATE INDEX ix_agendamentos_paciente ON agendamentos (paciente_id);