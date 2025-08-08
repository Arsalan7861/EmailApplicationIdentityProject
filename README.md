# RentCarProject

Welcome to the Email Application! This project is designed to provide a secure mail application for users. Built using **.NET CORE MVC**, this application aims to make managing mails easy for users and
checks the messages whether it is toxic or not automatically with AI model.

## Table of Contents
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Installation](#installation)
- [Contributing](#contributing)
- [License](#license)

## Features
- **Profile**: 
  - A clean and intuitive interface that allows users to easily edit their profile.
  - Provides quick access to user's info.

- **Mail Management**:
  - Users can send and recieve mails.
  - Checking the toxicity of the emails with AI.

- **Roling System**:
  - Roles can be assigned to users for more security.

- **User Authentication**:
  - Secure login and registration system for users and administrators.
  - Password recovery options to help users regain access to their accounts.

- **Responsive Design**:
  - The application is designed to work seamlessly on various devices, including desktops, tablets, and smartphones.
  - Adapts layout and functionality for optimal user experience across different screen sizes..

  ![Inbox](https://github.com/Arsalan7861/EmailApplicationIdentityProject/blob/master/screenshots/inbox.png)

  ![Compose Email](https://github.com/Arsalan7861/EmailApplicationIdentityProject/blob/master/screenshots/compose.png)
  ![Email Control](https://github.com/Arsalan7861/EmailApplicationIdentityProject/blob/master/screenshots/email-control.png)
- **UI**:
  - UI part is under development and It will be developed soon.  

## Technology Stack
- **MVC:** .NET Core 9
- **Database:** SQL Server Management Studio 20

## Installation

To set up the project locally, follow these steps:

1. Clone the repository:
   ```bash
   git clone https://github.com/Arsalan7861/EmailApplicationIdentityProject.git
   ```
   
2. Navigate to the project directory:
   ```bash
   cd EmailApplicationIdentityProject
   ```

3. Open the project in Visual Studio:
   - Navigate to the server folder and open the solution file (.sln) in Visual Studio.

5. Restore the dependencies for the server-side:
   ```bash
   dotnet restore
   ```
   ### Important:
     Don't forget to update the **connection string**.
     Also take you own API KEY from Hugging Face for the AI model to work.

## Contributing

Contributions are welcome! If you have suggestions for improvements or new features, feel free to create an issue or submit a pull request.

1. Fork the repository.
2. Create your feature branch:
   ```bash
   git checkout -b feature/YourFeature
   ```
3. Commit your changes:
   ```bash
   git commit -m 'Add some feature'
   ```
4. Push to the branch:
   ```bash
   git push origin feature/YourFeature
   ```
5. Open a pull request.

## License

This project does not have a specified license. Please feel free to use it for personal or educational purposes.
