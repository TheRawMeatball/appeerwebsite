function onSignIn(googleUser, ref) {
    var token = googleUser.getAuthResponse().id_token;
    ref.invokeMethodAsync("GoogleSigninCallback", token);
}

window.googleSigninRender = (ref) => {
    gapi.signin2.render('my-signin2', {
        'scope': 'profile email',
        'width': 109.5,
        'height': 36.5,
        'longtitle': false,
        'theme': 'dark',
        'onsuccess': (googleUser) => onSignIn(googleUser, ref),
    });
}

window.googleSignOut = async () => {
    try {
        var auth2 = gapi.auth2.getAuthInstance();
        await auth2.signOut()
    } catch (error) { }
}