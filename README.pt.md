# Kisbolt

<p align="center">
  <img src="docs/assets/kisbolt-header.png" alt="Kisbolt — servidores e dados em movimento" width="900" />
</p>

[English](README.md) | [Deutsch](README.de.md) | [Français](README.fr.md) | [Español](README.es.md) | **Português** | [中文](README.zh.md) | [日本語](README.ja.md) | [Magyar](README.hu.md)

Uma loja de software para desktop (Avalonia 12 + [KD.Avalonia.Rice](https://www.nuget.org/packages/KD.Avalonia.Rice)).
Não tem catálogo próprio: adicionam-se repositórios do GitHub, GitLab (gitlab.com ou auto-hospedado) e Codeberg
(Forgejo/Gitea), o Kisbolt escolhe entre as suas versões (releases) o pacote adequado ao sistema e instala-o com o
**instalador do sistema operativo anfitrião**:

| Sistema | Pacote | Instalador |
|---------|--------|------------|
| Debian / Ubuntu e derivados | `.deb` | `pkexec apt-get` |
| Fedora / RHEL | `.rpm` | `pkexec dnf` |
| openSUSE / SLES | `.rpm` | `pkexec zypper` |
| Windows | `.msi`, `.exe` | `msiexec` / o próprio instalador (UAC) |

## Funcionamento

- **Adicionar um repositório**: a loja só aceita um repositório se ele tiver uma versão com um pacote para o sistema
  atual (formato + arquitetura). Caso contrário, informa o motivo.
- **Ecrã principal**: em cima as aplicações instaladas, em baixo as instaláveis, 20 por página em cada grupo.
- **Atualizações**: procurar e instalar atualizações por aplicação instalada ou para todas de uma vez.
- **Estado**: ao iniciar, o Kisbolt verifica se os pacotes instalados ainda existem (removido externamente → a aplicação
  volta para a lista das instaláveis).
- **Tokens de API**: token opcional por host (limites de pedidos, repositórios privados) nas definições.
- **Idioma**: a interface está disponível em inglês (predefinição), alemão, francês, espanhol, português, chinês,
  japonês e húngaro; muda-se nas definições e aplica-se de imediato.
- **Dados**: SQLite, `~/.local/share/kisbolt/kisbolt.db` (Windows: `%LOCALAPPDATA%\kisbolt\kisbolt.db`), ícones ao lado na
  pasta `icons`.

## Metadados `.kisbolt`

Um repositório pode indicar no ficheiro `.kisbolt/kisbolt.json` como deve aparecer na loja:

```json
{
  "name": "My App",
  "version": "1.2.3",
  "author": "Alguém",
  "icon": "icon.png"
}
```

- Todos os campos são opcionais; sem nome → nome do repositório, sem autor → proprietário do repositório.
- `icon` é um caminho relativo à pasta `.kisbolt` (png, jpg, webp, ico, bmp) e não pode apontar para fora dela.
- A loja volta a ler o ficheiro ao adicionar o repositório e em cada procura de atualizações.
- Se o utilizador definir um ícone próprio para uma aplicação, este é guardado localmente e a loja deixa de ler a
  pasta `.kisbolt` (reverte-se com a opção de menu *Repor ícone (.kisbolt)*).

## Desenvolvimento

```sh
dotnet build
dotnet test
dotnet run --project src/Kisbolt.App
```

- `src/Kisbolt.Core` – modelo de dados (EF Core + SQLite), clientes de forjas, seleção de pacotes, instaladores,
  `StoreService`.
- `src/Kisbolt.App` – interface Avalonia (MVVM, CommunityToolkit.Mvvm).
- `src/Kisbolt.*/Resources/Strings*.resx` – textos da interface e das mensagens, um ficheiro por idioma
  (`Strings.resx` é o inglês).
- `tests/Kisbolt.Core.Tests` – testes xUnit.

Nova migração: `dotnet ef migrations add <Nome> --project src/Kisbolt.Core --output-dir Data/Migrations`.
