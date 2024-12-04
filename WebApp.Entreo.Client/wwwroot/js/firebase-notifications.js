window.firebaseNotifications = {
    checkPermission: async function () {
        try {
            // Check if notifications are supported
            if (!('Notification' in window)) {
                console.error('This browser does not support notifications');
                return 'denied';
            }

            // Check if Firebase Messaging is supported
            if (!firebase.messaging.isSupported()) {
                console.error('Firebase Messaging is not supported');
                return 'denied';
            }

            // Check if we're in a secure context (HTTPS or localhost)
            if (!window.isSecureContext) {
                console.error('Notifications require a secure context (HTTPS or localhost)');
                return 'denied';
            }

            return Notification.permission;
        } catch (error) {
            console.error('Error checking permission:', error);
            return 'denied';
        }
    },

    requestPermission: async function () {
        console.log('In requestPermission');
        try {
            // Check if notifications are supported
            if (!('Notification' in window)) {
                console.error('This browser does not support notifications');
                return 'denied';
            }

            // Check if Firebase Messaging is supported
            if (!firebase.messaging.isSupported()) {
                console.error('Firebase Messaging is not supported');
                return 'denied';
            }

            const messaging = firebase.messaging();
            
            // Request permission from the user
            const permission = await Notification.requestPermission();
            console.log('Permission request result:', permission);

            if (permission === 'granted') {
                // Get the token only if permission is granted
                const token = await messaging.getToken({
                    vapidKey: 'BJVjvvevU-dZDMNGWElrU8xdEGA3wR73jedNnCOwiCpveHKzc9Ht0kmE64nQQZA-XIaCIBO1Zup0w1B8UHLOLTo'
                });
                console.log('FCM Token:', token);
            }

            return permission;
        } catch (error) {
            console.error('Error requesting permission:', error);
            return 'denied';
        }
    },

    getToken: async function () {
        try {
            const messaging = firebase.messaging();
            const token = await messaging.getToken({
                vapidKey: 'BJVjvvevU-dZDMNGWElrU8xdEGA3wR73jedNnCOwiCpveHKzc9Ht0kmE64nQQZA-XIaCIBO1Zup0w1B8UHLOLTo'
            });
            console.log('Token retrieved:', token);
            return token;
        } catch (error) {
            console.error('Error getting token:', error);
            return null;
        }
    },

    initializeMessaging: function (dotNetHelper, messageCallback, tokenCallback) {
        try {
            const messaging = firebase.messaging();

            // Handle incoming messages when the app is in the foreground
            messaging.onMessage((payload) => {
                console.log('Message received in foreground:', payload);
                
                const message = {
                    title: payload.notification?.title || 'New Notification',
                    body: payload.notification?.body || '',
                    icon: payload.notification?.icon || '/icon-192.png',
                    type: payload.data?.type || 'default',
                    data: payload.data || {}
                };

                // Show browser notification
                if (Notification.permission === 'granted') {
                    new Notification(message.title, {
                        body: message.body,
                        icon: message.icon,
                        data: message.data
                    });
                }

                // Invoke .NET method
                dotNetHelper.invokeMethodAsync(messageCallback, message);
            });

            // Handle token refresh
            messaging.onTokenRefresh(async () => {
                try {
                    const newToken = await messaging.getToken({
                        vapidKey: 'BJVjvvevU-dZDMNGWElrU8xdEGA3wR73jedNnCOwiCpveHKzc9Ht0kmE64nQQZA-XIaCIBO1Zup0w1B8UHLOLTo'
                    });
                    console.log('Token refreshed:', newToken);
                    dotNetHelper.invokeMethodAsync(tokenCallback, newToken);
                } catch (error) {
                    console.error('Error refreshing token:', error);
                }
            });

            console.log('Firebase Messaging initialized successfully');
        } catch (error) {
            console.error('Error initializing Firebase Messaging:', error);
        }
    },

    // Helper function to check if the browser supports notifications
    checkBrowserSupport: function () {
        const support = {
            notifications: 'Notification' in window,
            serviceWorker: 'serviceWorker' in navigator,
            pushManager: 'PushManager' in window,
            firebaseMessaging: firebase.messaging.isSupported()
        };

        console.log('Browser support:', support);
        return support;
    },

    // Helper function to handle notification clicks
    handleNotificationClick: function (event) {
        console.log('Notification clicked:', event);
        
        // Focus or open the window
        if (event.target.data && event.target.data.url) {
            window.focus();
            window.location.href = event.target.data.url;
        }
    },

    //Kan weg?
    registerServiceWorker: function (event) {
        console.log('Going to register the Service Worker', event);

        // Register service worker for background notifications
        if ('serviceWorker' in navigator) {
            navigator.serviceWorker
                .register('/firebase-messaging-sw.js')
                .then(function (registration) {
                    console.log('Service Worker registered with scope:', registration.scope);
                })
                .catch(function (error) {
                    console.error('Service Worker registration failed:', error);
                });
        }
}
};
