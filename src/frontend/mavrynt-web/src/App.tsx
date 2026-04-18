import { useState } from "react";

export default function App() {
  const [result, setResult] = useState("not called");

  async function callApi() {
    const response = await fetch("/api/ping");
    const data = await response.json();
    setResult(data.message);
  }

  return (
      <div style={{ padding: 24, fontFamily: "Arial, sans-serif" }}>
        <h1>Mavrynt Web</h1>
        <button onClick={callApi}>Call API</button>
        <p>Result: {result}</p>
      </div>
  );
}