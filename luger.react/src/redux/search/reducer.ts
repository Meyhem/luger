import dayjs from 'dayjs'
import { getType, Reducer } from 'typesafe-actions'
import { SearchActions } from './actions'
import { BucketSearchState, SearchState } from './types'

export const initialState: SearchState = {}

export const bucketInitialState = {
  filter: {
    from: dayjs().subtract(15, 'minutes').toJSON(),
    to: new Date().toJSON(),
    levels: [],
    page: 0,
    pageSize: 20
  },
  loading: false,
  logs: []
}

function setBucketState(state: SearchState, bucket: string, bucketState: Partial<BucketSearchState>): SearchState {
  return { ...state, [bucket]: { ...state[bucket], ...bucketState } }
}

export const searchReducer: Reducer<SearchState, SearchActions> = (state = initialState, action) => {
  switch (action.type) {
    case getType(SearchActions.resetFilter):
      return setBucketState(state, action.payload.bucket, bucketInitialState)
    case getType(SearchActions.setFilter):
      return setBucketState(state, action.payload.bucket, { filter: action.payload.filter })
    case getType(SearchActions.setLogs):
      return setBucketState(state, action.payload.bucket, {
        logs: action.payload.logs
      })
    case getType(SearchActions.resetLogs):
      return setBucketState(state, action.payload.bucket, {
        logs: []
      })
    default:
      return state
  }
}
