# XgpLib.SyncService.UnitTests

Este projeto contém os testes unitários para a solução XgpLib.SyncService.

## 🧪 Estrutura de Testes

```
XgpLib.SyncService.UnitTests/
├── Application/
│   ├── Genres/
│   │   ├── Commands/
│   │   │   └── SyncGenresCommandHandlerTests.cs
│   │   └── Queries/
│   │       └── GetGenreByIdQueryResponseHandlerTests.cs
│   └── UseCases/
│       └── PublishMessageUseCaseTests.cs
├── Domain/
│   └── Entities/
│       ├── AuditableEntityTests.cs
│       ├── BaseEntityTests.cs
│       ├── GameTests.cs
│       └── GenreTests.cs
└── Helpers/
    └── TestDataBuilder.cs
```

## 📦 Tecnologias e Pacotes

- **xUnit** - Framework de testes
- **Moq** - Biblioteca de mocking
- **FluentAssertions** - Assertions fluentes e legíveis
- **Bogus** - Geração de dados fake para testes
- **coverlet.collector** - Coleta de cobertura de código
- **ReportGenerator** - Geração de relatórios de cobertura em HTML

## 🚀 Como Executar os Testes

### Executar todos os testes

```powershell
dotnet test
```

### Executar testes com output detalhado

```powershell
dotnet test --logger "console;verbosity=detailed"
```

### Executar testes de um arquivo específico

```powershell
dotnet test --filter "FullyQualifiedName~GenreTests"
```

### Executar um teste específico

```powershell
dotnet test --filter "FullyQualifiedName~GenreTests.Genre_ShouldInitializeWithDefaultValues"
```

## 📊 Cobertura de Código

### Executar testes com cobertura

```powershell
dotnet test --collect:"XPlat Code Coverage"
```

### Gerar relatório HTML de cobertura

```powershell
# 1. Executar testes e coletar cobertura
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults

# 2. Gerar relatório HTML
reportgenerator `
  -reports:"./TestResults/**/coverage.cobertura.xml" `
  -targetdir:"./TestResults/CoverageReport" `
  -reporttypes:"Html;Badges"

# 3. Abrir o relatório
Start-Process ./TestResults/CoverageReport/index.html
```

### Script completo (PowerShell)

Salve este script como `run-tests-with-coverage.ps1`:

```powershell
# Limpar resultados anteriores
if (Test-Path ./TestResults) {
    Remove-Item -Recurse -Force ./TestResults
}

# Executar testes com cobertura
Write-Host "Executando testes..." -ForegroundColor Green
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults

# Gerar relatório HTML
Write-Host "Gerando relatório de cobertura..." -ForegroundColor Green
reportgenerator `
  -reports:"./TestResults/**/coverage.cobertura.xml" `
  -targetdir:"./TestResults/CoverageReport" `
  -reporttypes:"Html;Badges;JsonSummary"

# Exibir resumo
Write-Host "`nRelatório de cobertura gerado em: ./TestResults/CoverageReport/index.html" -ForegroundColor Cyan
Write-Host "Abrindo relatório no navegador..." -ForegroundColor Green

# Abrir relatório
Start-Process ./TestResults/CoverageReport/index.html
```

Execute com:

```powershell
.\run-tests-with-coverage.ps1
```

## 📈 Métricas de Cobertura

O projeto visa manter as seguintes métricas de cobertura:

- **Cobertura de Linha**: ≥ 80%
- **Cobertura de Branch**: ≥ 70%
- **Cobertura de Método**: ≥ 85%

## 🔧 Padrões de Teste

### Nomenclatura

- **Classe de Teste**: `{ClasseEmTeste}Tests`
- **Método de Teste**: `{Método}_{Cenário}_{ResultadoEsperado}`

Exemplo:
```csharp
public class GenreTests
{
    [Fact]
    public void Genre_ShouldInitializeWithDefaultValues()
    {
        // Teste
    }
}
```

### Estrutura AAA

Todos os testes seguem o padrão **Arrange-Act-Assert**:

```csharp
[Fact]
public void HandleAsync_WithValidGenres_ShouldSyncSuccessfully()
{
    // Arrange - Preparar os dados e mocks
    var command = new SyncGenresCommand();
    var igdbGenres = TestDataBuilder.IgdbGenreFaker().Generate(5);

    // Act - Executar a ação
    var result = await _handler.HandleAsync(command, CancellationToken.None);

    // Assert - Verificar o resultado
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeTrue();
}
```

### Uso de Test Data Builders

Utilize a classe `TestDataBuilder` para criar dados de teste:

```csharp
// Gerar uma entidade Genre fake
var genre = TestDataBuilder.GenreFaker().Generate();

// Gerar múltiplas entidades
var genres = TestDataBuilder.GenreFaker().Generate(10);

// Gerar DTOs
var igdbGenre = TestDataBuilder.IgdbGenreFaker().Generate();
var igdbGames = TestDataBuilder.IgdbGameFaker().Generate(5);
```

## 🧩 Categorias de Testes

### Testes de Entidades (Domain)

Testam as entidades de domínio:
- Inicialização com valores padrão
- Herança correta
- Propriedades e validações

### Testes de Use Cases (Application)

Testam a lógica de negócio:
- Fluxos de sucesso
- Tratamento de erros
- Validações
- Logging

### Testes de Handlers (Application)

Testam os Command e Query Handlers:
- Mapeamento de dados
- Integração com repositórios
- Tratamento de exceções
- Logging

## 📝 Exemplo de Teste Completo

```csharp
using XgpLib.SyncService.Application.UseCases;

namespace XgpLib.SyncService.UnitTests.Application.UseCases;

public class PublishMessageUseCaseTests
{
    private readonly Mock<ILogger<PublishMessageUseCase>> _loggerMock;
    private readonly Mock<IMessageBrokerService> _messageBrokerServiceMock;
    private readonly PublishMessageUseCase _useCase;

    public PublishMessageUseCaseTests()
    {
        _loggerMock = new Mock<ILogger<PublishMessageUseCase>>();
        _messageBrokerServiceMock = new Mock<IMessageBrokerService>();
        _useCase = new PublishMessageUseCase(
            _loggerMock.Object,
            _messageBrokerServiceMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldPublishMessageSuccessfully()
    {
        // Arrange
        var request = new PublishMessageRequest
        {
            Topic = "test-topic",
            Message = "test message"
        };

        _messageBrokerServiceMock
            .Setup(x => x.PublishMessageAsync(
                request.Topic,
                request.Message,
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();

        _messageBrokerServiceMock.Verify(
            x => x.PublishMessageAsync(
                request.Topic,
                request.Message,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
```

## 🔍 Verificação de Mocks

Use `Verify` do Moq para garantir que métodos foram chamados:

```csharp
// Verificar que foi chamado exatamente uma vez
_repositoryMock.Verify(
    x => x.GetGenreById(genreId, It.IsAny<CancellationToken>()),
    Times.Once);

// Verificar que nunca foi chamado
_serviceMock.Verify(
    x => x.SomeMethod(),
    Times.Never);

// Verificar quantidade específica de chamadas
_loggerMock.Verify(
    x => x.Log(...),
    Times.Exactly(3));
```

## 📚 Recursos Adicionais

- [xUnit Documentation](https://xunit.net/)
- [Moq Quickstart](https://github.com/moq/moq4/wiki/Quickstart)
- [FluentAssertions Documentation](https://fluentassertions.com/introduction)
- [Bogus Documentation](https://github.com/bchavez/Bogus)
- [Coverlet Documentation](https://github.com/coverlet-coverage/coverlet)

## 🤝 Contribuindo

Ao adicionar novos testes:

1. Siga os padrões de nomenclatura
2. Use AAA (Arrange-Act-Assert)
3. Mantenha os testes simples e focados
4. Teste casos de sucesso e de erro
5. Verifique a cobertura de código
6. Adicione testes para edge cases
7. Use Test Data Builders para dados fake

---

**Mantido por**: Time de Desenvolvimento XgpLib
**Última atualização**: Novembro 2025
