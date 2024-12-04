(function () {    
    const applicationServerPublicKey = 'BPMrSEv1DfVgx_3Cpca6p04SOnZRYZjs11wWq_PYnuR-EbAYQMbRO-oZ_9eZq53ja1QTMdBl01XCBxrVXJi4Imk';

    window.blazorPushNotifications = {
        requestSubscription: async () => {
            if (!await requestNotificationPermission()) {
                return
            }

            const registration = await navigator.serviceWorker.getRegistration();
            const existingSubscription = await registration.pushManager.getSubscription();
            if (!existingSubscription) {
                const subscription = await subscribe(registration);
                if (subscription) {
                    return {
                        url: subscription.endpoint,
                        p256dh: arrayBufferToBase64(subscription.getKey('p256dh')),
                        auth: arrayBufferToBase64(subscription.getKey('auth'))
                    };
                }
            }
            else {
                return {
                    url: existingSubscription.endpoint,
                    p256dh: arrayBufferToBase64(existingSubscription.getKey('p256dh')),
                    auth: arrayBufferToBase64(existingSubscription.getKey('auth'))
                };
            }
        },

        cancelSubscription: async () => {
            const registration = await navigator.serviceWorker.getRegistration()
            const existingSubscription = await registration.pushManager.getSubscription()

            if (existingSubscription) {
                const result = await existingSubscription.unsubscribe()
                //console.log(result)
                return result;
            }
            else { return true; }
        }
    };

    async function subscribe(registration) {
        try {
            return await registration.pushManager.subscribe({
                userVisibleOnly: true,
                applicationServerKey: applicationServerPublicKey
            });
        } catch (error) {
            if (error.name === 'NotAllowedError') {
                return null;
            }
            throw error;
        }
    }

    function arrayBufferToBase64(buffer) {
        // https://stackoverflow.com/a/9458996
        var binary = '';
        var bytes = new Uint8Array(buffer);
        var len = bytes.byteLength;
        for (var i = 0; i < len; i++) {
            binary += String.fromCharCode(bytes[i]);
        }
        return window.btoa(binary);
    }

    async function requestNotificationPermission() {
        switch (Notification.permission) {
            case 'default':
                const permission = await Notification.requestPermission()
                return (permission === 'granted')
            case 'denied':
                //console.log('Push通知が拒否されている')
                return false
            case 'granted':
                //console.log('Push通知が許可されている')
                return true
        }
    }
})();
