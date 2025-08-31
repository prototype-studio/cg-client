mergeInto(LibraryManager.library, {
  CustomHubConnection_Create: function (urlPtr, tokenPtr) {
    const url = UTF8ToString(urlPtr);
    const token = UTF8ToString(tokenPtr);

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(url, {
        accessTokenFactory: () => token
      })
      .build();

    const id = Date.now().toString();
    window._hubConnections = window._hubConnections || {};
    window._hubConnections[id] = connection;

    // Closed event
    connection.onclose((err) => {
      if (typeof unityInstance !== "undefined") {
        unityInstance.SendMessage("SignalRBridge", "OnClosed", err ? err.message : "");
      }
    });

    // ✅ Allocate string for Unity properly
    return stringToNewUTF8(id);
  },

  CustomHubConnection_Connect: function (idPtr) {
    const id = UTF8ToString(idPtr);
    const conn = window._hubConnections[id];
    if (conn) {
      conn.start().catch(err => {
        console.error("SignalR connect error:", err);
      });
    }
  },

  CustomHubConnection_On: function (idPtr, methodNamePtr) {
    const id = UTF8ToString(idPtr);
    const methodName = UTF8ToString(methodNamePtr);
    const conn = window._hubConnections[id];
    if (conn) {
      conn.on(methodName, (msg) => {
        if (typeof unityInstance !== "undefined") {
          unityInstance.SendMessage("SignalRBridge", "OnMessage", JSON.stringify({
            method: methodName,
            payload: msg
          }));
        }
      });
    }
  },

  CustomHubConnection_Stop: function (idPtr) {
    const id = UTF8ToString(idPtr);
    const conn = window._hubConnections[id];
    if (conn) {
      conn.stop().then(() => {
        delete window._hubConnections[id];
      });
    }
  }
});
