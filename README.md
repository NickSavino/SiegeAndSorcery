
<a id="readme-top"></a>





<br />
<div align="center">
  <a href="https://github.com/desobob11/SDLDoom">
  </a>

  <h3 align="center">Retail Renaissance :heavy_dollar_sign: :chart_with_upwards_trend: </h3>

  <p align="center">
    A product database management application! Track items by trending sales volumes, shipments from suppliers, customer transactions, and more!
  </p>
</div>





## About The Project

This application was developed as a course project for a database course during my time at the University of Calgary. Retail Renaissance is comprised of a **Node.js** server that communicates with a **MySQL** database, and a **React.js** frontend.

By interacting with the web application, users trigger stored procedures in the **MySQL** database in real time! Business users can view trending products, manage inventory volumes, manage customer profiles, and order shipments to desired store branches.

### Built With

![NodeJS](https://img.shields.io/badge/node.js-6DA55F?style=for-the-badge&logo=node.js&logoColor=white)
![React](https://img.shields.io/badge/react-%2320232a.svg?style=for-the-badge&logo=react&logoColor=%2361DAFB)
![MySQL](https://img.shields.io/badge/mysql-4479A1.svg?style=for-the-badge&logo=mysql&logoColor=white)
![CSS3](https://img.shields.io/badge/css3-%231572B6.svg?style=for-the-badge&logo=css3&logoColor=white)
![HTML5](https://img.shields.io/badge/html5-%23E34F26.svg?style=for-the-badge&logo=html5&logoColor=white)

<p align="right">(<a href="#readme-top">back to top</a>)</p>




## Getting Started


## Prerequisites

The following programs are required to run Retail Renaissance

1. A web browser of your choice
2. [Python](https://www.python.org/downloads/)
3. [MySQL (including MySQL Workbench)](https://dev.mysql.com/downloads/workbench/)
4. [Node.js](https://nodejs.org/en)

## Installation

Begin by cloning this repository to your local machine.

**MySQL**
1. Ensure that a MySQL server is running on your machine
2. Open MySQL workbench and navigate to the [Data Import wizard](https://dev.mysql.com/doc/workbench/en/wb-admin-export-import-management.html)
3. Select the **rr_bootstrap** folder included in this repository
4. The **rr** schema will not be built!
5. In the **projectnode/sql** folder, execute each **.sql** script in the **rr** schema on **MySQL Workbench**
6. All of the required stored procedures will be created!

**Add Yourself as an Administrator**
1. In **MySQL Workbench**, execute the following command to create an admin user:

    `CALL add_user('root', 'root', 'root', 'root@root.ca', 1, 1);`

**Populating the database**
This project includes a Python script to populate the database with the help of [Faker](https://faker.readthedocs.io/en/master/).
1. Execute **datafill.py**
2. If the script fails to complete, please install each package shown in the error output using [Pip](https://pypi.org/project/pip/).

**Launching the Node.js Server**
1. Navigate to the **projectnode/server.js** and **projectnode/client.js** files
2. Adjust the constant paramaters in each script to fit your MySQL configuration
3. When ready to run the server, simply execute the following commands to install required **npm** packages and launch the server:

    `npm install`
    `node server.js`

**Launching the React.js Frontend**
1. Navigate to the **app** directory
2. Execute the following commands to install all packages and start the frontend:

    `npm install`
    `node start`

**Finish**

You should now be greeted with the Login screen as your web browser boots up. Enter your user credentials and have fun!
<p align="right">(<a href="#readme-top">back to top</a>)</p>







## Contact

Desmond O'Brien -- desmondobrien01@outlook.com

Victor Campos -- victor.goitia.campos@gmail.com 


Project Link: [https://github.com/desobob11/RetailRenaissance](https://github.com/desobob11/RetailRenaissance)

<p align="right">(<a href="#readme-top">back to top</a>)</p>

