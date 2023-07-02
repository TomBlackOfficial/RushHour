from fastapi import FastAPI
import json

app = FastAPI()


@app.post("/")
def add_score(username: str, score: int):
    file = open("scores.json")

    data = json.load(file)
    if username in data:
        if score <= data[username]:
            file.close()
            return "ok"

    data[username] = score
    file.close()

    print(data)

    with open("scores.json", "w") as file:
        json_object = json.dumps(data)
        file.write(json_object)

    return "ok"


@app.get("/")
def get_top_players():
    file = open("scores.json")

    data = json.load(file)
    sorted_data = sorted(data.items(), key=lambda x : x[1], reverse=True)

    top_players = {}
    for i in range(5):
        top_players[sorted_data[i][0]] = sorted_data[i][1]

    return top_players
