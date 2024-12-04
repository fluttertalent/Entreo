// Import the functions you need from the SDKs you need
import { initializeApp } from "/js/firebase-app.js";
import { getAnalytics } from "/js/firebase-analytics.js";

// Your web app's Firebase configuration
// For Firebase JS SDK v7.20.0 and later, measurementId is optional
const firebaseConfig = {
    apiKey: "AIzaSyBC_Bo66IBuELMrx-c-U2BsKdJAX1VMDkE",
    authDomain: "web-app-entreo.firebaseapp.com",
    projectId: "web-app-entreo",
    storageBucket: "web-app-entreo.firebasestorage.app",
    messagingSenderId: "161982635120",
    appId: "1:161982635120:web:c51d0b30ce34604bf5a8b5",
    measurementId: "G-LFHPN0P2JB"
};

// Initialize Firebase
app = initializeApp(firebaseConfig);
analytics = getAnalytics(app);


messaging = firebase.messaging();

messaging.onBackgroundMessage((payload) => {
    console.log('Background message received:', payload);

    const notificationTitle = payload.notification.title;
    const notificationOptions = {
        body: payload.notification.body,
        icon: payload.notification.icon,
        data: payload.data
    };

    return self.registration.showNotification(notificationTitle, notificationOptions);
});

window.firebaseApp = app;
window.firebaseMessaging = messaging;
onno = "kkk";