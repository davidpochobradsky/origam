import { computed, observable } from "mobx";
import { onRefreshChatrooms } from "model/actions/Chatrooms/onRefreshChatrooms";
import { getApi } from "model/selectors/getApi";
import { PeriodicLoader } from "utils/PeriodicLoader";

export class Chatrooms {
  *getChatroomsList() {
    const api = getApi(this);
    const chatrooms = yield api.getChatroomList();
    this.items = chatrooms;
  }

  loader = new PeriodicLoader(onRefreshChatrooms(this));

  @observable items: any[] = [];
  @computed get totalItemCount() {
    return this.items.map((item) => item.unreadMessageCount).reduce((a, b) => a + b, 0);
  }

  get sortedItems() {
    return this.items;
  }

  *startTimer(refreshIntervalMs: number) {
    if (localStorage.getItem("debugNoPolling_chatrooms")) return;
    if (localStorage.getItem("debugPollingMs_chatrooms")) {
      refreshIntervalMs = parseInt(localStorage.getItem("debugPollingMs_chatrooms") || "30000");
    }
    yield* this.loader.start(refreshIntervalMs);
  }

  parent?: any;
}
