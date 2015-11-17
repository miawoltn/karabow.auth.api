function ViewModel() {
    var self = this;

    var tokenKey = 'accessToken';
    var userKey = 'user';

    self.result = ko.observable();
    self.user = ko.observable();

    self.registerEmail = ko.observable();
    self.registerName = ko.observable();
    self.registerID = ko.observable();
    self.registerPassword = ko.observable();
    self.registerPassword2 = ko.observable();

    self.loginEmail = ko.observable();
    self.loginPassword = ko.observable();

    function showError(jqXHR) {
           // alert(jqXHR.status + '\n' + '\n' + jqXHR.statusText + '\n' + jqXHR.responseText);

            var response = null;
            var errors = [];
            var errorsString = "";
           
            if (jqXHR.status == 400) {
                try {
                    response = JSON.parse(jqXHR.responseText);
                }
                catch (e) {
                    errorsString = 'Try again';
                    alert(errorsString);
                }
            }
            if (response != null) {
                var modelState = response.modelState;
                for (var key in modelState) {
                    if (modelState.hasOwnProperty(key)) {
                        errorsString = (errorsString == "" ? "" : errorsString + "<br/>") + modelState[key];
                        errors.push(modelState[key]);//list of error messages in an array
                    }
                }
                if(response['error_description'] != null)
                {
                    errorsString = (errorsString == "" ? "" : errorsString + "<br/>") + response['error_description'];
                    errors.push(response['error_description']);
                }
            }
            if (errorsString != "")
            {
                $.each(errors, function (index, value) {
                    $('#error').html("<li>"+value+"</li>");
                });
            }
            if (jqXHR.status == 401)
            {
                response = JSON.parse(jqXHR.responseText);
                    $('#error').html("<li>" + response["message"] + "</li>");
                
            }
          
      // self.result(jqXHR);
    }

    self.callApi = function () {
        self.result('');

        var token = sessionStorage.getItem(tokenKey);
        var user = sessionStorage.getItem(userKey);
        var headers = {};
        if (token) {
            headers.Authorization = 'Bearer ' + token;
        }
        $.ajax({
            type: 'GET',
            url: '/api/accounts/user/'+user,
            headers: headers
        }).done(function (data) {

                //alert(ok);
                $('#error').append("<li>" + data["userName"] + "</li>");
                $('#error').append("<li>" + data["fullName"] + "</li>");
                $('#error').append("<li>" + data["email"] + "</li>");
                     
        }).fail(showError);
    }

    self.register = function () {
        $('#error').html("<li> Please wait... </li>");
       
        var data = {
            FullName: self.registerName(),
            Email: self.registerEmail(),
            UserName: self.registerID(),
            Password: self.registerPassword(),
            ConfirmPassword: self.registerPassword2()
        };

        $.ajax({
            type: 'POST',
            url: '/api/accounts/create',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(data)
        }).done(function (jqXHR) {
               // alert('ok');
                $('#error').html("<li> A confirmation email has been sent to your inbox. Thanks for registering. </li>");
            
        }).fail(showError);
    }

    self.login = function () {
        self.result('');
        var username = self.loginEmail();
        var loginData = {
            grant_type: 'password',
            username: self.loginEmail(),
            password: self.loginPassword()
        };

        $.ajax({
            type: 'POST',
            url: '/oauth/token',
            data: loginData
        }).done(function (data) {
            self.user(data.userName);
            // Cache the access token in session storage.
            sessionStorage.setItem(tokenKey, data.access_token);
            sessionStorage.setItem(userKey, username);
            //self.result( + "\n" + data.access_token);
            $('#error').html("<li>Welcome "+username+", your are logged in</li>" + "<li>"+data.access_token+"</li>");
        }).fail(showError);
    }

    self.logout = function () {
        //self.user('');        
        sessionStorage.removeItem(tokenKey)
        $('#error').html("Your are now logged in ");
    }
}

var app = new ViewModel();
ko.applyBindings(app);