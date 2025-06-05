# 📊 Financial API

> **API REST completa em .NET 8** para cotações de ações e gestão de watchlist  
> Desenvolvida em 5 horas como demonstração de skills .NET Core

[![.NET](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com/)
[![Entity Framework](https://img.shields.io/badge/Entity%20Framework-Core-blue)](https://docs.microsoft.com/en-us/ef/core/)
[![SQLite](https://img.shields.io/badge/Database-SQLite-green)](https://www.sqlite.org/)
[![Swagger](https://img.shields.io/badge/API%20Docs-Swagger-orange)](https://swagger.io/)

## 🎯 Visão Geral

Uma API REST moderna e completa que demonstra as principais tecnologias e padrões do ecossistema .NET, incluindo cotações de ações em tempo real, gestão de watchlist personalizada e análise técnica avançada.

### ✨ Funcionalidades Principais

- 📈 **Cotações de Ações** - Dados em tempo real (simulados)
- 📊 **Histórico de Preços** - Até 30 dias de dados históricos
- 📋 **Watchlist Personalizada** - CRUD completo para ações favoritas
- 🔍 **Análise Técnica** - Média móvel, RSI, volatilidade e recomendações
- 💾 **Persistência** - Banco SQLite com Entity Framework
- ⚡ **Cache Inteligente** - Sistema de cache em memória para performance
- 📚 **Documentação** - Swagger UI completo e interativo

## 🛠️ Tecnologias Utilizadas

### Backend
- **.NET 8** - Framework principal
- **ASP.NET Core Web API** - API REST
- **Entity Framework Core** - ORM
- **SQLite** - Banco de dados
- **Memory Caching** - Cache em memória

### Documentação & Ferramentas
- **Swagger/OpenAPI** - Documentação automática
- **Data Annotations** - Validações
- **Dependency Injection** - Injeção de dependência
- **Logging** - Sistema de logs integrado

## 🚀 Começando

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Git
- IDE (Visual Studio, VS Code, Rider)

### Instalação

1. **Clone o repositório**
   ```bash
   git clone https://github.com/javabetatester/FinancialAPI.git
   cd FinancialAPI
   ```

2. **Restaurar dependências**
   ```bash
   dotnet restore
   ```

3. **Executar a aplicação**
   ```bash
   dotnet run
   ```

4. **Acessar a documentação**
   - Swagger UI: http://localhost:5127
   - Health Check: http://localhost:5127/health

## 📋 Endpoints da API

### 📊 Cotações
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/api/stocks/{symbol}` | Cotação atual de uma ação |
| `GET` | `/api/stocks/{symbol}/history?days=7` | Histórico de preços |

### 📝 Watchlist
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/api/watchlist` | Listar watchlist com preços atuais |
| `POST` | `/api/watchlist` | Adicionar ação à watchlist |
| `DELETE` | `/api/watchlist/{id}` | Remover ação da watchlist |

### 🔍 Análise Técnica
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/api/analysis/{symbol}` | Análise técnica completa |
| `GET` | `/api/analysis/{symbol}/simple` | Análise simplificada |

### 🏥 Utilitários
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/health` | Status da aplicação |
| `GET` | `/info` | Informações da API |

## 💡 Exemplos de Uso

### Obter cotação da Apple
```bash
GET http://localhost:5127/api/stocks/AAPL
```

**Resposta:**
```json
{
  "symbol": "AAPL",
  "name": "Apple Inc.",
  "price": 182.50,
  "change": 2.30,
  "changePercent": 1.28,
  "lastUpdated": "2025-06-04T15:30:00Z"
}
```

### Adicionar à watchlist
```bash
POST http://localhost:5127/api/watchlist
Content-Type: application/json

{
  "symbol": "MSFT",
  "name": "Microsoft Corporation"
}
```

### Análise técnica detalhada
```bash
GET http://localhost:5127/api/analysis/AAPL?days=14
```

**Resposta:**
```json
{
  "symbol": "AAPL",
  "currentPrice": 182.50,
  "analysis": {
    "movingAverage": 180.25,
    "trend": "Up",
    "trendPercentage": 1.25,
    "volatility": 3.45,
    "recommendation": "Buy"
  },
  "technicalIndicators": {
    "rsi": 65.8,
    "support": 175.20,
    "resistance": 185.90
  }
}
```

## 🏗️ Arquitetura

### Estrutura do Projeto
```
FinancialAPI/
├── Controllers/           # Endpoints da API
│   ├── StocksController.cs
│   ├── WatchlistController.cs
│   └── AnalysisController.cs
├── Services/              # Lógica de negócio
│   ├── IStockService.cs
│   ├── StockService.cs
│   └── YahooFinanceService.cs
├── Models/                # Modelos de dados
│   ├── Stock.cs
│   ├── WatchlistItem.cs
│   └── StockAnalysis.cs
├── Data/                  # Contexto do banco
│   └── AppDbContext.cs
└── Program.cs             # Configuração da aplicação
```

### Padrões Implementados
- **Repository Pattern** - Acesso a dados
- **Service Layer** - Lógica de negócio
- **Dependency Injection** - Inversão de controle
- **Caching Strategy** - Performance otimizada
- **Error Handling** - Tratamento robusto de erros

## 🧪 Testes

### Teste manual via Swagger
1. Acesse http://localhost:5127
2. Explore os endpoints interativamente
3. Teste com diferentes símbolos: AAPL, MSFT, GOOGL, TSLA

### Validações implementadas
- ✅ Validação de símbolos de ações
- ✅ Tratamento de erros robusto
- ✅ Cache com TTL configurável
- ✅ Logs detalhados para debugging

## 📊 Performance

### Cache Strategy
- **Stocks**: 5 minutos TTL
- **History**: 10 minutos TTL  
- **Analysis**: 15 minutos TTL

### Dados de exemplo
A API vem pré-configurada com dados de exemplo:
- Apple (AAPL)
- Microsoft (MSFT)
- Alphabet (GOOGL)

## 🔧 Configuração

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=stocks.db"
  },
  "Urls": "http://localhost:5127",
  "YahooFinance": {
    "CacheTimeoutMinutes": 5
  }
}
```

### Variáveis de ambiente
```bash
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://localhost:5127
```

## 🚦 Status do Projeto

### ✅ Implementado
- [x] CRUD completo de Watchlist
- [x] Cotações com cache inteligente
- [x] Análise técnica avançada
- [x] Documentação Swagger completa
- [x] Validações e tratamento de erros
- [x] Dados de exemplo pré-carregados

### 🔄 Melhorias Futuras
- [ ] Integração com API real (Yahoo Finance, Alpha Vantage)
- [ ] Autenticação JWT
- [ ] Testes unitários e de integração
- [ ] Rate limiting
- [ ] Dockerização
- [ ] Deploy automatizado

## 🤝 Contribuindo

1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/nova-feature`)
3. Commit suas mudanças (`git commit -m 'Add: nova feature'`)
4. Push para a branch (`git push origin feature/nova-feature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto é open source e está disponível sob a [MIT License](LICENSE).

## 👨‍💻 Autor

**Desenvolvido por:** [Seu Nome]  
**Contato:** [seu.email@exemplo.com]  
**LinkedIn:** [Seu LinkedIn]  
**GitHub:** [@javabetatester](https://github.com/javabetatester)

---

⭐ Se este projeto te ajudou, deixe uma estrela no repositório!

**Desenvolvido em 5 horas como demonstração de skills em .NET 8** 🚀