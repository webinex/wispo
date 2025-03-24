import { FirebaseOptions, initializeApp } from "firebase/app";
import {
  getMessaging,
  onBackgroundMessage,
  Unsubscribe,
} from "firebase/messaging/sw";
import { WISPO_FCM_MESSAGE_CONFIG_TYPE } from "../constants";

let firebaseUnsubscribe: Unsubscribe | null = null;

export const initializeWispoFCMSw = () => {
  self.addEventListener("message", (event) => {
    if (!!event.data && event.data.type === WISPO_FCM_MESSAGE_CONFIG_TYPE) {
      subscribeToFCMMessages(event.data.payload);
    }
  });
};

const subscribeToFCMMessages = (options: FirebaseOptions) => {
  const firebaseApp = initializeApp(options);
  const messaging = getMessaging(firebaseApp);

  unsubscribeFromFCMMessages();

  firebaseUnsubscribe = onBackgroundMessage(messaging, (payload) => {
    console.debug(
      "[firebase-messaging-sw.js] Received background message ",
      payload
    );
  });

  console.info("Firebase messaging service worker is set up");
};

const unsubscribeFromFCMMessages = () => {
  firebaseUnsubscribe?.();
  firebaseUnsubscribe = null;
};
