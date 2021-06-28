# How to use

## Login

Application requires authentication to use. This is something that is
setup by the Admin as the users has to be assigned to a specific
location. Admin-user has a link `Lisää käyttäjä` in which a new user can
be created.

Application automatically navigates to login if the user is not
authenticated. After login user is redirected to main page.

## Main page

On load the main page shows the current week's predictions.

The predictions contains the following attributes:

  - Date of the prediction
  - Menu for the day (user can select another menu if the prefetched
    menu is not correct)
  - Predictions:
      - Number of diners
      - Total waste in kg
      - Line waste in kg
      - Plate waste in kg
      - Production waste in kg
  - Production:
      - An input field for how many diners food has been prepared for
  - Actual input fields
      - Number of all diners
      - Number of diners with special diet
      - Line waste in kg
      - Plate waste in kg
      - Production waste in kg
      - Optional comment (this is required if the waste is greater than
        configured maximum)

After modifying any of the fields the user can save the form. Saved
fields are used to calculate predictions.

### Selecting week and location

By default predictions are fetched for the current week and users first
location. These can be changed by accessing the inputs before
predictions. Date selection always selects the whole week. Location list
is populated with locations which user has access to.

## Report

For further analyze there's a link `Raportti` where user has access to
`PowerBI` -report based on the inputted waste amounts.
