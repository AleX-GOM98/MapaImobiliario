# MapaImobiliario

🗺️ **MapaImobiliario** é uma plataforma que centraliza imóveis à venda de diversos sites e exibe suas localizações aproximadas em um mapa interativo.

## 🔍 Objetivo
- Coletar imóveis de diferentes portais como VivaReal, OLX, ZapImóveis etc.
- Exibir os resultados por região geográfica (bairro, zona ou cidade).
- Permitir busca por mapa com filtros (valor, tipo, região).

## 🧱 Estrutura
\`\`\`
/api           → API backend em .NET
/scraper       → Scripts de scraping (Python)
/frontend      → Interface Web (HTML + JS)
/.github       → Workflows de CI (GitHub Actions)
\`\`\`

## 🚀 Tecnologias
- .NET 6+ (API REST)
- Python 3 (Web scraping)
- Leaflet.js ou Mapbox (Mapa)
- GitHub Actions (CI/CD)
- GitHub Projects (Kanban de tarefas - opcional)

## 📦 Como rodar localmente
\`\`\`bash
# Clone o projeto
git clone https://github.com/seu-usuario/MapaImobiliario.git

# Entre no projeto
cd MapaImobiliario
\`\`\`

> Instruções específicas de cada parte estão nas subpastas \`api/\`, \`scraper/\`, \`frontend/\`.

## 📌 Roadmap
- [x] Estrutura inicial
- [ ] Scraper do VivaReal
- [ ] API de listagem
- [ ] Mapa interativo
- [ ] Filtros por região e preço

## 📄 Licença
MIT License - veja o arquivo \`LICENSE\`
EOF
