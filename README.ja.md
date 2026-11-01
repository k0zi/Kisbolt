# Kisbolt

<p align="center">
  <img src="docs/assets/kisbolt-header.png" alt="Kisbolt — サーバーと流れるデータ" width="900" />
</p>

[English](README.md) | [Deutsch](README.de.md) | [Français](README.fr.md) | [Español](README.es.md) | [Português](README.pt.md) | [中文](README.zh.md) | **日本語** | [Magyar](README.hu.md)

デスクトップ向けソフトウェアストア（Avalonia 12 + [KD.Avalonia.Rice](https://www.nuget.org/packages/KD.Avalonia.Rice)）。
独自のカタログは持たず、GitHub・GitLab（gitlab.com またはセルフホスト）・Codeberg（Forgejo/Gitea）のリポジトリを
追加すると、Kisbolt がそのリリースの中からシステムに合ったパッケージを選び、**ホスト OS のインストーラー**で
インストールします：

| システム | パッケージ | インストーラー |
|----------|------------|----------------|
| Debian / Ubuntu および派生ディストリビューション | `.deb` | `pkexec apt-get` |
| Fedora / RHEL | `.rpm` | `pkexec dnf` |
| openSUSE / SLES | `.rpm` | `pkexec zypper` |
| Windows | `.msi`、`.exe` | `msiexec` / インストーラー自身（UAC） |

## 動作

- **リポジトリの追加**：現在のシステム（形式 + アーキテクチャ）に合ったパッケージを含むリリースがある場合にのみ
  ストアに追加されます。そうでない場合は理由が表示されます。
- **メイン画面**：上にインストール済み、下にインストール可能なアプリが表示され、各グループ 20 件ずつページ送りされます。
- **アップデート**：インストール済みアプリごと、またはすべてまとめてアップデートを確認・インストールできます。
- **状態**：起動時に Kisbolt はインストール済みパッケージがまだ存在するかを確認します（外部で削除された場合 →
  インストール可能な一覧に戻ります）。
- **API トークン**：設定でホストごとに任意のトークン（レート制限、プライベートリポジトリ用）を指定できます。
- **言語**：UI は英語（既定）、ドイツ語、フランス語、スペイン語、ポルトガル語、中国語、日本語、ハンガリー語に対応しています。
  設定で切り替えると、すぐに反映されます。
- **データ**：SQLite、`~/.local/share/kisbolt/kisbolt.db`（Windows：`%LOCALAPPDATA%\kisbolt\kisbolt.db`）、アイコンは同じ場所の
  `icons` フォルダーに保存されます。

## `.kisbolt` メタデータ

リポジトリは `.kisbolt/kisbolt.json` ファイルで、ストアでの表示方法を指定できます：

```json
{
  "name": "My App",
  "version": "1.2.3",
  "author": "誰か",
  "icon": "icon.png"
}
```

- すべてのフィールドは任意です。名前がない場合 → リポジトリ名、作者がない場合 → リポジトリの所有者。
- `icon` は `.kisbolt` フォルダーからの相対パス（png、jpg、webp、ico、bmp）で、フォルダーの外を指すことはできません。
- ストアはリポジトリの追加時と、アップデートの確認のたびにファイルを読み直します。
- ユーザーがアプリに独自のアイコンを設定すると、それはローカルに保存され、以降ストアは `.kisbolt` フォルダーを
  読まなくなります（メニュー項目 *アイコンをリセット (.kisbolt)* で元に戻せます）。

## 開発

```sh
dotnet build
dotnet test
dotnet run --project src/Kisbolt.App
```

- `src/Kisbolt.Core` – データモデル（EF Core + SQLite）、フォージクライアント、パッケージ選択、インストーラー、`StoreService`。
- `src/Kisbolt.App` – Avalonia UI（MVVM、CommunityToolkit.Mvvm）。
- `src/Kisbolt.*/Resources/Strings*.resx` – UI とメッセージのテキスト。言語ごとに 1 ファイル（`Strings.resx` は英語）。
- `tests/Kisbolt.Core.Tests` – xUnit テスト。

新しいマイグレーション：`dotnet ef migrations add <名前> --project src/Kisbolt.Core --output-dir Data/Migrations`。
