# matensa
This is a mini project that provides Users APIs and Balance.

To Run the project:
1- clone the repository:
* git clone https://github.com/ramitto1111/matensa.git

2- import the SQL database:
* You will find the database on the root with the name "matensa.bacpac".
* Open SQL server management stodeo, right click on the Databases and click on "Import Data-tier Application" then follow the steps.

3- To test the APIs:
* Open the project on Visual Studio.
* Click on the Run button.
* a new browser page opens with the Swagger UI interface which provide an easy way to test the APIs.



Available APIs:
____________________

Users:
-------
- GET /api/Users/List              (This API retrieves all the users).
- GET /api/Users/{id}              (This API retrieves the user with provided Id).
- POST /api/Users/Create           (This API creates a new user).
- PUT /api/Users/{id}              (This API updates the user with provided Id).
- DELETE /api/Users/{id}           (This API deletes the user with provided Id).
- POST /api/Users/Balance/{id}     (This API add Balance a new user to the user with provided Id).

- POST /api/Users/Login            (This is the Login API that return Token).

TranserFunds:
-------------
- POST /apiTransferFunds           (This API transfer amount from user to another).
this API I tested using POSTMAN 
to send the Token throug headers ex: Bearer Token_value