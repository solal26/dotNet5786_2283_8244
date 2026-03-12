This Windows-based application is a comprehensive delivery management system designed to coordinate logistics between customers, parcels, and couriers. The project demonstrates the transition from complex business logic to a high-performance graphical interface, focusing on real-world supply chain scenarios.

Architecture (N-Tier)
The system is built using a Multitier (3-Layer) Architecture to ensure strict separation of concerns:

Data Access Layer (DAL): Manages persistent data storage using XML DOM.

Business Logic Layer (BL): Handles core logistics rules, delivery constraints, and data processing via LINQ.

Presentation Layer (PL): A reactive UI built with WPF and the MVVM pattern.

Technical Stack
Language & Framework: C# / .NET

UI: WPF (XAML) with Advanced Data Binding and DataTemplates.

Data: XML serialization and LINQ for complex collection filtering.

Patterns: Singleton (Data Access), Observer (UI updates), Factory Method, and Dependency Injection.

Key Functionalities
Delivery Tracking: Complete management of parcels, drones/couriers, and customer assignments.

Operational Logic: Implementation of "Design by Contract" to validate logistics constraints at every step.

Performance: Multi-threading and asynchronous tasks to keep the interface fluid during data processing.

Dynamic Interface: Specialized XAML Styles and Resources for a professional user experience.
