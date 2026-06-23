# Fırın API

.NET 8 Web API + Vue 3 frontend ile fırın yönetim sistemi.

## Özellikler

- Ekmek, pasta, müşteri ve sipariş yönetimi
- PostgreSQL (Docker) + EF Core migrations
- FluentValidation, JWT Auth, CORS, Health check
- Sayfalama: `GET /api/breads?page=1&size=10`
- Vue 3 admin paneli ve herkese açık menü

---

## Gereksinimler

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Node.js 20+](https://nodejs.org/) (frontend için)

---

## Hızlı başlangıç

### 1. PostgreSQL (Docker)

> **Not:** Bilgisayarınızda zaten PostgreSQL çalışıyorsa Docker portu `5434` kullanılır (5432/5433 dolu olabilir).

```powershell
cd "C:\Users\ferdi\Fırın Api"
docker compose up -d
```

### 2. Gizli ayarlar (User Secrets — sadece ilk kurulumda)

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5434;Database=firin_db;Username=firin_user;Password=firin123"
dotnet user-secrets set "Jwt:Key" "FirinApiSuperSecretKeyForDevelopment_Min32Chars!"
dotnet user-secrets set "Seed:AdminPassword" "Admin123!"
```

Şablon: `secrets.example.json` ve `.env.example`

### 3. API

```powershell
dotnet run
```

| Adres | Açıklama |
|-------|----------|
| http://localhost:5220/swagger | API dokümantasyonu |
| http://localhost:5220/health | Sistem durumu |

### 4. Vue frontend

```powershell
cd frontend
npm install
npm run dev
```

Tarayıcı: http://localhost:5173

---

## Admin girişi

| Alan | Değer (varsayılan dev) |
|------|------------------------|
| Kullanıcı | `admin` |
| Şifre | `Admin123!` (User Secrets'tan) |

---

## Sayfalama

Tüm liste endpoint'leri sayfalama destekler:

```
GET /api/breads?page=1&size=10
GET /api/cakes?page=1&size=10
GET /api/customers?page=1&size=10
GET /api/orders?page=1&size=10
```

Yanıt örneği:

```json
{
  "items": [...],
  "page": 1,
  "pageSize": 10,
  "totalCount": 25,
  "totalPages": 3
}
```

---

## Production ayarları

Şifreleri **asla** `appsettings.json`'a yazmayın. Ortam değişkeni kullanın:

```bash
ConnectionStrings__DefaultConnection=Host=...;Port=5434;Database=firin_db;Username=...;Password=...
Jwt__Key=uzun-ve-gizli-anahtar-en-az-32-karakter
Seed__AdminPassword=guclu-sifre
```

---

## Test

```powershell
dotnet test
```

---

## CI/CD

GitHub Actions (`.github/workflows/ci.yml`): build, test, frontend build.
