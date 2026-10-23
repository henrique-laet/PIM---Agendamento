-- VitaCare - Dados de exemplo para desenvolvimento e apresentação

INSERT INTO especialidades (nome, icone) VALUES
    ('Clínica Geral', 'stethoscope'),
    ('Cardiologia',   'heart-pulse'),
    ('Dermatologia',  'scan-face'),
    ('Pediatria',     'baby'),
    ('Ortopedia',     'bone'),
    ('Oftalmologia',  'eye');

INSERT INTO medicos (nome, crm, especialidade_id, bio) VALUES
    ('Dra. Helena Duarte',  'CRM-SP 100001', (SELECT id FROM especialidades WHERE nome = 'Clínica Geral'), 'Atendimento acolhedor e foco em prevenção.'),
    ('Dr. Marcelo Tavares', 'CRM-SP 100002', (SELECT id FROM especialidades WHERE nome = 'Clínica Geral'), 'Check-ups completos e acompanhamento contínuo.'),
    ('Dr. Rafael Campos',   'CRM-SP 100003', (SELECT id FROM especialidades WHERE nome = 'Cardiologia'),   'Especialista em saúde do coração.'),
    ('Dra. Beatriz Lima',   'CRM-SP 100004', (SELECT id FROM especialidades WHERE nome = 'Dermatologia'),  'Cuidados com a pele em todas as idades.'),
    ('Dra. Camila Ribeiro', 'CRM-SP 100005', (SELECT id FROM especialidades WHERE nome = 'Pediatria'),     'Atenção especial para crianças e famílias.'),
    ('Dr. André Moreira',   'CRM-SP 100006', (SELECT id FROM especialidades WHERE nome = 'Ortopedia'),     'Tratamento de lesões e reabilitação.'),
    ('Dra. Larissa Prado',  'CRM-SP 100007', (SELECT id FROM especialidades WHERE nome = 'Oftalmologia'),  'Exames de vista e saúde ocular.');

-- Gera horários para os próximos 14 dias, de segunda a sexta
INSERT INTO horarios_disponiveis (medico_id, data_hora)
SELECT m.id, (CURRENT_DATE + d.dia) + h.hora
FROM medicos m
CROSS JOIN generate_series(1, 14) AS d(dia)
CROSS JOIN (VALUES ('09:00'::time), ('10:00'), ('11:00'),
                   ('14:00'), ('15:00'), ('16:00')) AS h(hora)
WHERE EXTRACT(ISODOW FROM CURRENT_DATE + d.dia) < 6;