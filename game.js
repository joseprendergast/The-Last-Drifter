const transcript = document.querySelector("#transcript");
const inventoryEl = document.querySelector("#inventory");
const objectiveEl = document.querySelector("#objective");
const resetButton = document.querySelector("#reset");
const verbButtons = [...document.querySelectorAll(".verb")];
const hotspots = [...document.querySelectorAll(".hotspot")];
const rainCanvas = document.querySelector("#rain");
const rainContext = rainCanvas.getContext("2d");

const initialState = {
  verb: "look",
  selectedItem: null,
  inventory: [],
  flags: {
    sawNote: false,
    foundCoin: false,
    calledNumber: false,
    openedLocker: false,
    readPostcard: false,
  },
};

let state = structuredClone(initialState);
let rainDrops = [];

const items = {
  note: {
    label: "Torn note",
    description: "A rain-softened scrap with three words left: CALL BAY 17.",
  },
  coin: {
    label: "Bus token",
    description: "Old brass, stamped with a route that no longer runs.",
  },
  postcard: {
    label: "Postcard",
    description: "A faded motel card. On the back: EAST, WHEN THE LIGHTS FAIL.",
  },
};

const objectives = [
  {
    test: () => !state.flags.sawNote,
    text: "Find out why the depot sign keeps buzzing every time lightning hits.",
  },
  {
    test: () => !state.inventory.includes("coin"),
    text: "The note points to Bay 17. Search the depot for something useful.",
  },
  {
    test: () => !state.flags.calledNumber,
    text: "Use the bus token on the payphone.",
  },
  {
    test: () => !state.flags.openedLocker,
    text: "A voice mentioned locker 8. Find what was left for you.",
  },
  {
    test: () => true,
    text: "The road east is open. This first slice is complete.",
  },
];

function writeLine(speaker, text) {
  const line = document.createElement("p");
  line.className = "line";
  line.innerHTML = `<strong>${speaker}</strong> ${text}`;
  transcript.appendChild(line);
  transcript.scrollTop = transcript.scrollHeight;
}

function setVerb(verb) {
  state.verb = verb;
  state.selectedItem = null;
  verbButtons.forEach((button) => {
    button.classList.toggle("is-active", button.dataset.verb === verb);
  });
  renderInventory();
}

function addItem(itemName) {
  if (state.inventory.includes(itemName)) return false;
  state.inventory.push(itemName);
  renderInventory();
  renderObjective();
  return true;
}

function removeItem(itemName) {
  state.inventory = state.inventory.filter((item) => item !== itemName);
  if (state.selectedItem === itemName) state.selectedItem = null;
  renderInventory();
}

function renderInventory() {
  inventoryEl.innerHTML = "";

  if (state.inventory.length === 0) {
    const empty = document.createElement("div");
    empty.className = "inventory-empty";
    empty.textContent = "Empty pockets";
    inventoryEl.appendChild(empty);
    return;
  }

  state.inventory.forEach((itemName) => {
    const button = document.createElement("button");
    button.type = "button";
    button.textContent = items[itemName].label;
    button.classList.toggle("is-selected", state.selectedItem === itemName);
    button.addEventListener("click", () => {
      if (state.verb === "use") {
        state.selectedItem = state.selectedItem === itemName ? null : itemName;
        renderInventory();
        writeLine("You:", state.selectedItem ? `Ready to use ${items[itemName].label}.` : "You lower your hand.");
        return;
      }

      writeLine("You:", items[itemName].description);
    });
    inventoryEl.appendChild(button);
  });
}

function renderObjective() {
  objectiveEl.textContent = objectives.find((objective) => objective.test()).text;
}

function handleHotspot(action) {
  const handler = actions[action]?.[state.verb] || actions[action]?.look;
  handler();
  renderObjective();
}

const actions = {
  note: {
    look() {
      state.flags.sawNote = true;
      writeLine("You:", "The scrap is wedged under a loose tile. CALL BAY 17. Same numbers as the sign.");
    },
    take() {
      state.flags.sawNote = true;
      if (addItem("note")) {
        writeLine("You:", "You fold the torn note into your coat before the rain finishes it off.");
      } else {
        writeLine("You:", "You already took the note.");
      }
    },
    use() {
      writeLine("You:", "The note is a clue, not a tool.");
    },
    talk() {
      writeLine("You:", "Paper keeps secrets better than people.");
    },
  },
  bench: {
    look() {
      writeLine("You:", "A bench polished by years of waiting. Something brass glints below it.");
    },
    take() {
      if (addItem("coin")) {
        state.flags.foundCoin = true;
        writeLine("You:", "You fish a bus token from a puddle under the bench.");
      } else {
        writeLine("You:", "Only wet concrete is left under the bench.");
      }
    },
    use() {
      writeLine("You:", "You sit for half a second. The city immediately feels heavier.");
    },
    talk() {
      writeLine("You:", "You ask the bench if it has seen a missing woman. It creaks like it might have.");
    },
  },
  payphone: {
    look() {
      writeLine("You:", "The payphone hums without a dial tone. A scratched label reads: exact fare only.");
    },
    take() {
      writeLine("You:", "It is bolted deep into the depot wall.");
    },
    use() {
      if (state.selectedItem === "coin") {
        removeItem("coin");
        state.flags.calledNumber = true;
        writeLine("Phone:", "Bay 17. Locker 8. Then east before sunrise.");
        return;
      }

      if (state.inventory.includes("coin")) {
        writeLine("You:", "Select the bus token in your inventory, then use it on the payphone.");
        return;
      }

      writeLine("You:", "The coin slot waits with the patience of a trap.");
    },
    talk() {
      if (state.flags.calledNumber) {
        writeLine("Phone:", "Static. A breath. Then nothing.");
      } else {
        writeLine("You:", "You lift the receiver. The silence on the line feels occupied.");
      }
    },
  },
  locker: {
    look() {
      writeLine("You:", state.flags.calledNumber ? "Locker 8 is half open, like someone left in a hurry." : "A row of lockers. Number 8 has fresh scratches around the latch.");
    },
    take() {
      writeLine("You:", "The locker stays put.");
    },
    use() {
      if (!state.flags.calledNumber) {
        writeLine("You:", "The latch sticks. You need a reason to force it.");
        return;
      }

      if (!state.flags.openedLocker) {
        state.flags.openedLocker = true;
        addItem("postcard");
        writeLine("You:", "Locker 8 opens. Inside is a postcard, dry as bone.");
        return;
      }

      writeLine("You:", "Locker 8 is empty now.");
    },
    talk() {
      writeLine("You:", "You whisper the name from your dream. The locker gives no answer.");
    },
  },
  road: {
    look() {
      if (state.flags.openedLocker) {
        writeLine("You:", "The east road flickers under failing lamps. For the first time tonight, it looks like a way out.");
        return;
      }

      writeLine("You:", "The road east is drowned in rain. You are missing something before you leave.");
    },
    take() {
      writeLine("You:", "You cannot carry the road. You can only choose it.");
    },
    use() {
      if (state.flags.openedLocker) {
        writeLine("You:", "You step past Bay 17 and into the headlights of a bus that should not exist.");
        writeLine("System:", "End of the first playable slice.");
        return;
      }

      writeLine("You:", "Leaving now would turn this mystery into a weather report.");
    },
    talk() {
      writeLine("You:", "You call into the rain. Somewhere east, a horn answers.");
    },
  },
};

function resetGame() {
  state = structuredClone(initialState);
  transcript.innerHTML = "";
  setVerb("look");
  renderInventory();
  renderObjective();
  writeLine("System:", "Rain needles the depot roof. Bay 17 buzzes like a bad memory.");
  writeLine("You:", "Last stop. Empty pockets. One sign that will not stop glowing.");
}

function resizeRain() {
  const ratio = window.devicePixelRatio || 1;
  rainCanvas.width = Math.floor(rainCanvas.clientWidth * ratio);
  rainCanvas.height = Math.floor(rainCanvas.clientHeight * ratio);
  rainContext.setTransform(ratio, 0, 0, ratio, 0, 0);

  const count = Math.max(80, Math.floor(rainCanvas.clientWidth / 10));
  rainDrops = Array.from({ length: count }, () => ({
    x: Math.random() * rainCanvas.clientWidth,
    y: Math.random() * rainCanvas.clientHeight,
    length: 12 + Math.random() * 28,
    speed: 7 + Math.random() * 9,
    opacity: 0.16 + Math.random() * 0.22,
  }));
}

function drawRain() {
  rainContext.clearRect(0, 0, rainCanvas.clientWidth, rainCanvas.clientHeight);
  rainContext.lineWidth = 1;

  rainDrops.forEach((drop) => {
    rainContext.strokeStyle = `rgba(177, 215, 221, ${drop.opacity})`;
    rainContext.beginPath();
    rainContext.moveTo(drop.x, drop.y);
    rainContext.lineTo(drop.x - 5, drop.y + drop.length);
    rainContext.stroke();

    drop.y += drop.speed;
    drop.x -= 1.8;

    if (drop.y > rainCanvas.clientHeight + 40) {
      drop.y = -40;
      drop.x = Math.random() * rainCanvas.clientWidth;
    }
  });

  requestAnimationFrame(drawRain);
}

verbButtons.forEach((button) => {
  button.addEventListener("click", () => setVerb(button.dataset.verb));
});

hotspots.forEach((hotspot) => {
  hotspot.addEventListener("click", () => handleHotspot(hotspot.dataset.action));
});

resetButton.addEventListener("click", resetGame);
window.addEventListener("resize", resizeRain);

resizeRain();
drawRain();
resetGame();
