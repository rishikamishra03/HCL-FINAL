# Retail Ordering System - HCL Final Assessment

A full-stack retail ordering platform for food items (Pizzas, Cold Drinks, and Breads) built with **ASP.NET Core API** and **Angular**.

## 🚀 Features

- **User Authentication**: Secure Login and Registration using JWT (JSON Web Tokens).
- **Product Catalog**: Browse various categories (Pizza, Cold Drinks, Breads) with brand and packaging details.
- **Cart Management**: Add/remove items, update quantities, and persist cart state.
- **Loyalty Point System**:
  - Automatically applies a discount if the user has **500+ loyalty points**.
  - Dynamic display of discounts in the checkout summary.
- **Order History**: Users can view their past orders and order status.
- **Responsive Design**: Modern and sleek UI built with Vanilla CSS and Angular.
- **Inventory Tracking**: Stock quantities managed via the backend database.
- **Email Notifications**: Integrated email service for order confirmations.

---

## 🛠️ Technology Stack

### Backend
- **Framework**: ASP.NET Core 8.0
- **Database**: MySQL (RetailDB)
- **ORM**: Entity Framework Core
- **Security**: JWT Authentication, CORS enabled
- **Documentation**: Swagger/OpenAPI

### Frontend
- **Framework**: Angular 19
- **Styling**: Vanilla CSS (Post-modern aesthetics)
- **State Management**: RxJS & Angular Services
- **Validation**: Angular Reactive Forms

---

## 📁 Project Structure

```text
HCL-FINAL/
├── backend/
│   └── RetailAPI/          # ASP.NET Core Web API
│       ├── Controllers/    # API Endpoints
│       ├── Models/         # Entity Models
│       ├── Services/       # Business Logic (Auth, Cart, Email)
│       └── Data/           # DbContext and DB Configuration
├── frontend/
│   └── retail-ui/          # Angular Application
│       ├── src/app/
│       │   ├── components/ # UI Components (Home, Cart, Checkout, etc.)
│       │   ├── services/   # API Communication Logic
│       │   └── core/       # Guards and Interceptors
├── database/               # SQL Scripts for Schema and Seed Data
└── Readme/                 # Design documents and screenshots
```

---

## ⚙️ Setup & Installation

### 1. Database Setup
1. Ensure **MySQL Server** is running.
2. Create a database named `RetailDB`.
3. Execute the SQL scripts located in the `database/` folder in the following order:
   - `RetailOrderingDB.sql` (Schema)
   - `SeedData.sql` (Initial Data)
   - `FixImagesAndDuplicates.sql` (Optimizations)

### 2. Backend Setup
1. Navigate to the backend directory:
   ```bash
   cd backend/RetailAPI
   ```
2. Update the connection string in `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "server=localhost;database=RetailDB;user=root;password=YOUR_PASSWORD"
   }
   ```
3. Run the API:
   ```bash
   dotnet run
   ```
   The API will be available at `https://localhost:7193` (or check your console output).

### 3. Frontend Setup
1. Navigate to the frontend directory:
   ```bash
   cd frontend/retail-ui
   ```
2. Install dependencies:
   ```bash
   npm install
   ```
3. Start the development server:
   ```bash
   ng serve
   ```
4. Access the application at `http://localhost:4200`.

---

## 📸 Screenshots

*(Add your screenshots here manually or via the documentation folder)*
- **Home Page**: [View](./Readme/home_page.png)
- **Cart**: [View](./Readme/cart_page.png)
- **Order History**: [View](./Readme/orders.png)

---

## 📜 License
This project was developed as part of the HCL Final Assessment.
