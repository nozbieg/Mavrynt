import { useEffect } from "react";
import { useLocation } from "react-router";
import { useAnalytics } from "./index.ts";

/**
 * Fires `pageView` on every route change. Mount once near the top of the
 * tree (inside `<AnalyticsContext.Provider>` + `<Router>`). Cheap —
 * runs a single effect per navigation.
 */
export const useTrackPageView = (): void => {
  const analytics = useAnalytics();
  const location = useLocation();

  useEffect(() => {
    analytics.pageView(location.pathname + location.search);
  }, [analytics, location.pathname, location.search]);
};
