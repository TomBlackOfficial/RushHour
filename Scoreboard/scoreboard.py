from fastapi import FastAPI
from deta import Deta

app = FastAPI()
deta = Deta()

db = deta.Base("scores")


@app.post("/")
def add_score(username: str, score: int):
    if username == "key":
        return "bad username"

    data = db.get("scores")
    if data is None:
        data = {}

    if username in data:
        if score <= data[username]:
            return "ok"

    data[username] = score

    db.put(data, "scores")

    return "ok"


@app.get("/")
def get_top_players():
    data = db.get("scores")
    if data is None:
        data = {"key": "scores"}

    data.pop("key")

    sorted_data = sorted(data.items(), key=lambda x: x[1], reverse=True)

    top_players = []
    for i in range(min(len(sorted_data), 5)):
        top_players.append({"username": sorted_data[i][0], "score": sorted_data[i][1]})

    return {"scores": top_players}
