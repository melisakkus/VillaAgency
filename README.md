# VillaAgency

Language Options: [🇬🇧 English (this file)](README.md) | [🇹🇷 Türkçe](README.tr.md)

**A real estate/villa showcase and management platform built on MongoDB with a layered architecture.**

> 🧩 For technical architecture decisions, layer design, and rationale: **[ARCHITECTURE.md](ARCHITECTURE.md)**
>
> 🔗 **Live Demo:** _(link will be shared here once deployed)_
>
> 🔐 To explore the panel together or request a setup/demo: [Contact](#contact)

Built with ASP.NET Core 8 MVC, this project combines a customer-facing showcase site for a real estate agency with the content management panel that powers it, in a single solution.

---

## About the Project

VillaAgency brings together the two different experiences a real estate agency needs, in a single solution.

The **public side** is a showcase site where visitors can browse villa listings filtered by category, learn about the agency, find answers to frequently asked questions, and leave a request through a contact form. The **admin panel** is a separate, login-protected area where everything on that site (listings, banners, video content, FAQs, contact information, incoming messages) is managed on a daily basis.

Role-based authorization is applied in the admin panel: an **Admin** can add **Manager** accounts with limited privileges; these accounts can only perform the operations assigned to them (product and message management) and cannot access the remaining modules (Banner, Contact, Feature/FAQ, Video, User Management). Incoming messages are managed with an inbox logic that supports read/deleted states. Product listing is done with paging in view of a growing data volume; any unexpected errors that occur in the system are handled through a centralized error-handling mechanism instead of being shown to the user as raw stack traces.

This project was built to demonstrate how the basic skeleton of a web application — layering, validation, logging, error handling, authorization, data access — can be set up consistently. The rationale behind the architectural decisions taken, and the known limitations, are covered in detail in a separate document ([ARCHITECTURE.md](ARCHITECTURE.md)).

---

## Screenshots

> To add images: create a `docs/screenshots/` folder at the project root, place your images there, replace the file names below with your own, and add a `![Description](docs/screenshots/file-name.png)` line under each heading — GitHub will render these images inline in the README automatically.

**Public Homepage**
`docs/screenshots/public-home.png`

**Product / Villa Listing & Filtering**
`docs/screenshots/public-products.png`

**Product Detail Page**
`docs/screenshots/public-product-detail.png`

**Contact Form**
`docs/screenshots/public-contact.png`

**Admin Dashboard**
`docs/screenshots/admin-dashboard.png`

**Product Management (Admin)**
`docs/screenshots/admin-products.png`

**Message Inbox (All / Unread / Deleted)**
`docs/screenshots/admin-messages.png`

**User Management (Admin)**
`docs/screenshots/admin-users.png`

**Dark Mode Comparison**
`docs/screenshots/dark-mode.png`

**Error Page**
`docs/screenshots/error-page.png`

---

## Key Features

### Public Site

- **Product/Villa Listing:** Category-based filtering and server-side paging support.
- **Homepage Showcase:** A database-level random sampling query that ensures at least one listing from every category is shown on the homepage.
- **Modular Content Management:** Sections such as Banner, promotional video, features/FAQ, and contact information are built with independent ViewComponents managed from the admin panel.
- **AJAX-Based Forms:** Contact and question forms are submitted without a page reload; the user gets an instant success or error notification.

### Admin Panel

- **Role-Based Authorization:** Different access scopes between Admin and Manager roles (see the [architecture document](ARCHITECTURE.md#authentication-and-role-based-authorization) for the detailed permission table).
- **Product Management:** Category and status-based filtering, paging, one-click status update (Active / Sold / Rented / Archived).
- **Message Inbox:** All / Unread / Deleted tabs, independent paging per tab, mark-as-read, and recoverable (soft delete) deletion.
- **Dashboard:** Total/active/sold product counts, category distribution, and recent messages summarized on a single page using parallel queries.
- **User Management:** Temporarily suspending an account's access by toggling it active/inactive, without deleting it.
- **Server-Side Validation:** Field-level, descriptive error messages via FluentValidation.
- **Dark Mode:** Dark theme support based on CSS custom properties.

---

## Technology Stack

- **Backend:** .NET 8, ASP.NET Core MVC
- **Database:** MongoDB
- **Authentication:** ASP.NET Core Identity (on top of MongoDB)
- **Validation:** FluentValidation
- **Logging:** Serilog (Console + daily rolling file)
- **Object Mapping:** Mapster
- **Front End:** Bootstrap 5, jQuery, SweetAlert2

For a detailed list of which packages each layer uses and why: **[ARCHITECTURE.md](ARCHITECTURE.md#dependency-inventory-nuget-packages)**

---

## Related Project: Test Data Generator (Python)

A separate Python tool has also been built to populate the database with realistic test data: **[VillaAgency_DataGenerator-Python-](https://github.com/melisakkus/VillaAgency_DataGenerator-Python-/tree/main)**.

This tool uses `Faker` to generate 1,000 listings with category-specific logical constraints (room/bathroom count, price range, floor/parking information); it then bulk-updates image links on MongoDB Atlas via `bulk_write`, converts the price field from `float` to `int`, assigns listings a probability-weighted status (`Status`) along with a creation timestamp, and cleans up legacy fields no longer in use (`$unset`). In short, it's a helper migration/seed layer that lets VillaAgency be showcased with a production-like data volume instead of an empty database.

For details, see the [README.md](https://github.com/melisakkus/VillaAgency_DataGenerator-Python-/blob/main/README.md) file in that repository.

---

## License

This project does not currently carry an open-source license — all rights are reserved. Reviewing the code is welcome, but copying, commercial reuse, or redistribution requires prior permission.

## Contact

For questions, live demo requests, or collaboration proposals, you can reach me through the following channels:

- **GitHub:** [github.com/melisakkus](https://github.com/melisakkus)
- **LinkedIn:** _(https://www.linkedin.com/in/melisa-akkus-/)_
- **Email:** _(melisa.akkus01@gmail.com)_