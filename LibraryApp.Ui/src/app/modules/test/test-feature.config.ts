import { InjectionToken } from '@angular/core';

export interface TestFeatureConfig {
  title: string;
  description: string;
  pageSizeOptions: number[];
}

export const TEST_FEATURE_CONFIG = new InjectionToken<TestFeatureConfig>('TEST_FEATURE_CONFIG');
