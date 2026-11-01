# WE'RE MAKING FUN OF YOU

A first-person horror-comedy game about a live show that knows you're playing.

## Play the standalone browser edition

The playable game is in `browser/`. It does **not** require Unity, Unity Hub, a game-engine installation, build tools, or paid assets.

- Open `browser/index.html` in a modern desktop browser.
- Choose English or Türkçe.
- Click the game view to capture the mouse.
- **WASD** move, **mouse** look, **E** interact, **Shift** run, **Esc** pause.

For local hosting, run `python3 -m http.server 8000` from the repository root and visit `http://localhost:8000/browser/`.

## Game content

The browser edition contains eight connected chapters, bilingual dialogue/objectives, first-person raycast rendering, collectibles and switches, hostile pursuit sequences, optional hidden evidence, and three ending routes. The intended full-story target is approximately 60 minutes, depending on exploration and discovery.

1. The Warm-Up
2. Backstage Etiquette
3. The Archive of You
4. The Sound of Screams
5. Maintenance Nightmare
6. The Audience
7. Control / Alt / Delete
8. The Last Laugh

## Verification

Run:

```bash
python3 Tools/validate_browser_game.py
```

This checks the browser game's required files and key game systems. It is a source-integrity check, not a substitute for testing in multiple real browsers and screen sizes.

## Legacy Unity prototype

The earlier Unity prototype and its staged pull requests remain in the repository history for reference. The standalone browser edition is the current no-Unity playable target.
