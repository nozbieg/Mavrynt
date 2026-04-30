import { render, screen, waitFor } from "@testing-library/react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import FeatureFlagsPage from "./FeatureFlagsPage";
vi.mock("../lib/auth/useAdminAuth",()=>({useAdminAuth:()=>({accessToken:"t",logout:vi.fn()})}));
describe("FeatureFlagsPage",()=>{beforeEach(()=>vi.restoreAllMocks());it("shows loading then empty state",async()=>{vi.spyOn(globalThis,"fetch").mockResolvedValue({ok:true,status:200,json:async()=>[]} as Response);render(<FeatureFlagsPage />);expect(screen.getByText(/Loading/i)).toBeInTheDocument();await waitFor(()=>expect(screen.getByText(/No feature flags found/i)).toBeInTheDocument());});it("renders flags",async()=>{vi.spyOn(globalThis,"fetch").mockResolvedValue({ok:true,status:200,json:async()=>[{key:"k",name:"Flag",isEnabled:true}]} as Response);render(<FeatureFlagsPage />);await waitFor(()=>expect(screen.getByText(/Flag/)).toBeInTheDocument());});});
